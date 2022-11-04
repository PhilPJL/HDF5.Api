#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.Utils;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using static HDF5.Api.NativeMethods.H5A;
using System.Linq;
using CommunityToolkit.HighPerformance;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal static unsafe class H5AAdapter
{
    internal static void Close(H5Attribute attribute)
    {
        close(attribute).ThrowIfError();
    }

    internal static H5Attribute Create<T>(
        H5Object<T> h5Object,
        string name,
        H5Type type,
        H5Space space) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var creationPropertyList = CreateCreationPropertyList(CharacterSet.Utf8);

        long h;

#if NET7_0_OR_GREATER
        h = create(h5Object, name, type, space, creationPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = create(h5Object, nameBytesPtr, type, space, creationPropertyList);
        }
#endif

        return new H5Attribute(h);
    }

    internal static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int result;

#if NET7_0_OR_GREATER
        result = delete(h5Object, name);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            result = delete(h5Object, nameBytesPtr);
        }
#endif

        result.ThrowIfError();
    }

    internal static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int result;

#if NET7_0_OR_GREATER
        result = exists(h5Object, name).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            result = exists(h5Object, nameBytesPtr).ThrowIfError();
        }
#endif

        return result > 0;
    }

    internal static IEnumerable<string> GetAttributeNames<T>(H5Object<T> h5Object) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        ulong idx = 0;

        var names = new List<string>();

        iterate(h5Object, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero).ThrowIfError();

        return names;

        int Callback(long id, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            try
            {
                var name = info.cset switch
                {
#if NET7_0_OR_GREATER
                    H5T.cset_t.ASCII or H5T.cset_t.UTF8 => Marshal.PtrToStringUTF8(intPtrName),
#else
                    H5T.cset_t.ASCII or H5T.cset_t.UTF8 => MarshalHelpers.PtrToStringUTF8(intPtrName),
#endif
                    // Don't throw inside callback - see HDF docs
                    _ => string.Empty
                };

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (name != null)
                {
                    names.Add(name);
                }

                return 0;
            }
            catch
            {
                // Don't throw inside callback - see HDF docs
                return -1;
            }
        }
    }

    /// <summary>
    /// Get copy of property list used when creating the attribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    internal static H5AttributeCreationPropertyList GetCreationPropertyList(H5Attribute attribute)
    {
        return H5PAdapter.GetPropertyList(attribute, get_create_plist, h => new H5AttributeCreationPropertyList(h));
    }

    /// <summary>
    /// Create a new attribute creation property list
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding)
    {
        // TODO: ideally cache two CreationPropertyLists (Utf8/Ascii) and exclude them from handle tracking.
        return H5PAdapter.Create(H5P.ATTRIBUTE_CREATE, h => new H5AttributeCreationPropertyList(h)
        {
            CharacterEncoding = encoding
        });
    }

    internal static string GetName(H5Attribute attribute)
    {
#if NET7_0_OR_GREATER
        return MarshalHelpers.GetName(attribute,
            (long attr_id, Span<byte> name, nint size) => get_name(attr_id, size, name));
#else
        return MarshalHelpers.GetName(attribute,
            (long attr_id, byte* name, nint size) => get_name(attr_id, size, name));
#endif
    }

    internal static H5Space GetSpace(H5Attribute attribute)
    {
        return new H5Space(get_space(attribute));
    }

    internal static int GetStorageSize(H5Attribute attribute)
    {
        // NOTE: get_storage_size doesn't return an error (-1) if it fails
        return (int)get_storage_size(attribute);
    }

    internal static H5Type GetType(H5Attribute attribute)
    {
        return new H5Type(get_type(attribute));
    }

    internal static H5Attribute Open<T>(H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        long h = 0;

#if NET7_0_OR_GREATER
        h = open(h5Object, name);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(h5Object, nameBytesPtr);
        }
#endif

        return new H5Attribute(h);
    }

    #region Read

    internal static string ReadString(H5Attribute attribute)
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO: optionally convert UTF-8 to Ascii with <?> markers
        // TODO: generalise to NPoints >= 0

        if (count != 1 ||
            dims.Any(d =>
                d.UpperLimit >
                1)) // NOTE: dims.Count could be > 0 with count == 1 where we have an array of [1]..[1] with one element
        {
            throw new H5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        if (type.IsVariableLengthString())
        {
            if (count < 256 / sizeof(nint))
            {
                Span<nint> buffer = stackalloc nint[(int)count];
                return ReadVariableStrings(buffer);
            }

            // TODO: performance check
            //#if NET7_0_OR_GREATER
            //            using var spanOwner = SpanOwner<nint>.Allocate((int)count);
            //            return ReadVariableStrings(spanOwner.Span);
            //#else
            return ReadVariableStrings(new Span<nint>(new nint[count]));
            //#endif

            string ReadVariableStrings(Span<nint> buffer)
            {
                fixed (nint* bufferPtr = buffer)
                {
                    // IntPtr is a struct so no need to pin
                    var ptr = new IntPtr(bufferPtr);
                    try
                    {
                        read(attribute, type, ptr).ThrowIfError();

                        if (buffer[0] == 0)
                        {
                            // If the attribute was never written (or do we allow nulls?)
                            return string.Empty;
                        }
                        else
                        {
                            // NOTE: no way to retrieve size of variable length buffer.
                            // Only search for null up to a fixed length.
                            Span<byte> bytes = new((byte*)buffer[0], H5Global.MaxVariableLengthStringBuffer);
                            var nullTerminatorIndex = System.MemoryExtensions.IndexOf(bytes, (byte)0);
                            if (nullTerminatorIndex != -1)
                            {
                                return Encoding.UTF8.GetString((byte*)buffer[0], nullTerminatorIndex);
                            }
                            else
                            {
                                throw new H5Exception(
                                    $"Unable to locate end of string within first {H5Global.MaxVariableLengthStringBuffer} bytes." +
                                    " If required increase the value in {nameof(H5Global)}.{nameof(H5Global.MaxVariableLengthStringBuffer)}).");
                            }
                        }
                    }
                    finally
                    {
                        // TODO: check this really works
                        H5DAdapter.ReclaimVariableLengthMemory(type, space, (byte**)bufferPtr);
                    }
                }
            }
        }
        // ReSharper disable once RedundantIfElseBlock
        else
        {
            int storageSize = attribute.StorageSize;

#if NET7_0_OR_GREATER
            if (storageSize < 256)
            {
                Span<byte> buffer = stackalloc byte[storageSize + 1];
                return ReadString(buffer);
            }
            else
            {
                using var spanOwner = SpanOwner<byte>.Allocate(storageSize + 1);
                return ReadString(spanOwner.Span);
            }

            string ReadString(Span<byte> buffer)
            {
                read(attribute, type, buffer).ThrowIfError();

                var nullTerminatorIndex = System.MemoryExtensions.IndexOf(buffer, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
            }
#else
            var buffer = new byte[storageSize + 1];
            fixed (byte* bufferPtr = buffer)
            {
                read(attribute, type, bufferPtr).ThrowIfError();

                Span<byte> bytes = buffer;
                var nullTerminatorIndex = System.MemoryExtensions.IndexOf(bytes, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
            }
#endif
        }
    }

    internal static bool ReadBool(H5Attribute attribute)
    {
        using var type = attribute.GetH5Type();
        return Read<byte>(attribute, type) != default;
    }

    internal static DateTimeOffset ReadDateTimeOffset(H5Attribute attribute)
    {
        // TODO: optionally write ticks + offset or value.ToString("O")
        using var type = attribute.GetH5Type();
        using var expectedType = H5TAdapter.CreateDateTimeOffsetType();
        // TODO: sort out the type/expectedType/cls stuff
        var value = ReadImpl<_DateTimeOffset>(attribute, type, expectedType);

        return new DateTimeOffset(DateTime.FromBinary(value.DateTime), TimeSpan.FromMinutes(value.Offset));
    }

    internal static T Read<T>(H5Attribute attribute) where T : unmanaged
    {
        using var type = attribute.GetH5Type();
        return Read<T>(attribute, type);
    }

    internal static T Read<T>(H5Attribute attribute, H5Type type) where T : unmanaged
    {
        using var nativeType = H5Type.GetEquivalentNativeType<T>();
        return ReadImpl<T>(attribute, type, nativeType);
    }

    internal static T ReadEnum<T>(H5Attribute attribute, bool verifyType = false) where T : unmanaged, Enum
    {
        using var nativeType = H5TAdapter.ConvertDotNetEnumUnderlyingTypeToH5NativeType<T>();
        using var type = attribute.GetH5Type();

        if (verifyType)
        {
            using var enumType = H5Type.CreateEnumType<T>();
            if (!enumType.IsEqualTo(type))
            {
                throw new H5Exception($"{attribute.Name} does not have an equivalent HDF5 enumeration of {typeof(T)}.");
            }
        }

        return ReadImpl<T>(attribute, type, nativeType);
    }

    private static T ReadImpl<T>(H5Attribute attribute, H5Type type, H5Type expectedType) where T : unmanaged
    {
        using var space = attribute.GetSpace();

        long count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO: handle dims.Count > 0 where NPoints=1
        // TODO: generalise to NPoints >= 0

        if (count != 1 || dims.Count != 0)
        {
            throw new H5Exception("Attribute is not scalar.");
        }

        // TODO: fix for enums
        //if (!type.IsEqualTo(expectedType))
        //{
        //    throw new H5Exception($"Attribute is not of expected type.");
        //}

        var cls = type.GetClass();

        var expectedCls = H5TAdapter.GetClass(expectedType);

        if (cls != expectedCls)
        {
            throw new H5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int attributeStorageSize = attribute.StorageSize;
        int marshalSize = sizeof(T);

        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, marshalSize);

        T value = default;
        read(attribute, type, new IntPtr(&value)).ThrowIfError();
        return value;
    }

    #endregion

    #region Write
    
    internal static void Write<T>(H5Attribute attribute, T value) where T : unmanaged
    {
        using var type = attribute.GetH5Type();

        if (value is bool flag)
        {
            Write(attribute, type, flag.ToByte());
        }
        else
        {
            Write(attribute, type, value);
        }
    }

    internal static void Write(H5Attribute attribute, DateTimeOffset value)
    {
        // TODO: optionally write ticks + offset or value.ToString("O")
        var _value = new _DateTimeOffset
        {
            DateTime = value.DateTime.ToBinary(),
            Offset = (short)value.Offset.TotalMinutes
        };

        Write(attribute, _value);
    }

    internal static void Write<T>(H5Attribute attribute, H5Type type, T value) where T : unmanaged
    {
        var size = sizeof(T);
        int attributeStorageSize = attribute.StorageSize;
        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, size);

        Write(attribute, type, new IntPtr(&value));
    }

    internal static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        write(attribute, type, buffer).ThrowIfError();
    }

    internal static void Write(H5Attribute attribute, string value)
    {
        // TODO: handle array of strings

        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        if (count != 1 || dims.Count != 0)
        {
            throw new H5Exception("Attribute is not scalar.");
        }

        var characterSet = type.CharacterSet;

        // TODO: optionally throw if writing a string containing non-ASCII characters when characterSet = Ascii
        // TODO: optionally silently truncate to nearest character (not byte)

        var bytes = characterSet switch
        {
            // we absolutely need to add '\0' :)
            CharacterSet.Ascii => Encoding.ASCII.GetBytes(value + '\0'),
            CharacterSet.Utf8 => Encoding.UTF8.GetBytes(value + '\0'),
            _ => throw new InvalidEnumArgumentException($"Unknown CharacterSet:{characterSet}.")
        };

        if (type.IsVariableLengthString())
        {
            fixed (void* fixedBytes = bytes)
            {
                var stringArray = new IntPtr[] { new(fixedBytes) };

                fixed (void* stringArrayPtr = stringArray)
                {
                    Write(attribute, type, new IntPtr(stringArrayPtr));
                }
            }
        }
        else
        {
            int storageSize = attribute.StorageSize;

            if (bytes.Length > storageSize)
            {
                throw new ArgumentOutOfRangeException(
                    $"The string requires {bytes.Length} storage which is greater than the allocated fixed storage size of {storageSize} bytes.");
            }

            fixed (void* fixedBytes = bytes)
            {
                Write(attribute, type, new IntPtr(fixedBytes));
            }
        }
    }
    
    #endregion

    internal static H5Attribute CreateStringAttribute<T>(
        H5Object<T> h5Object, string name, int fixedStorageLength,
        CharacterSet cset, StringPadding padding) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var type = fixedStorageLength != 0
            ? H5TAdapter.CreateFixedLengthStringType(fixedStorageLength)
            : H5TAdapter.CreateVariableLengthStringType();

        type.CharacterSet = cset;
        type.StringPadding = padding;

        using var memorySpace = H5SAdapter.CreateScalar();
        return Create(h5Object, name, type, memorySpace);
    }
}
