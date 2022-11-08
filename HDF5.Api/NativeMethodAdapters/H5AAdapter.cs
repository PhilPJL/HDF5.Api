using HDF5.Api.Utils;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using static HDF5.Api.NativeMethods.H5A;
using HDF5.Api.H5Attributes;
using HDF5.Api.H5Types;

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

    internal static TA Create<T, TA, TT>(
        H5Object<T> h5Object, string name, Func<TT> typeCtor, Func<H5Space> spaceCtor, Func<long, TA> attributeCtor)
        where T : H5Object<T>
        where TA : H5Attribute
        where TT : H5Type
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var creationPropertyList = CreateCreationPropertyList(CharacterSet.Utf8);

        long h;

        using var type = typeCtor();
        using var space = spaceCtor();

#if NET7_0_OR_GREATER
        h = create(h5Object, name, type, space, creationPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = create(h5Object, nameBytesPtr, type, space, creationPropertyList);
        }
#endif

        return attributeCtor(h);
    }

    internal static TA CreateOrOpen<T, TA, TT>(
        H5Object<T> h5Object, bool exists, string name, Func<TT> typeCtor, Func<H5Space> spaceCtor, Func<long, TA> attributeCtor)
        where T : H5Object<T>
        where TA : H5Attribute
        where TT : H5Type
    {
        if (exists)
        {
            return Open(h5Object, name, attributeCtor);
        }
        else
        {
            return Create(h5Object, name, typeCtor, spaceCtor, attributeCtor);
        }
    }

    internal static TA CreateOrOpen<T, TA, TT>(
        H5Object<T> h5Object, string name, Func<TT> typeCtor, Func<H5Space> spaceCtor, Func<long, TA> attributeCtor)
        where T : H5Object<T>
        where TA : H5Attribute
        where TT : H5Type
    {
        return CreateOrOpen(h5Object, Exists(h5Object, name), name, typeCtor, spaceCtor, attributeCtor);
    }

    internal static H5StringAttribute CreateStringAttribute<T>(
        H5Object<T> h5Object, string name, int fixedStorageLength,
        CharacterSet cset, StringPadding padding) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        H5StringType typeCtor()
        {
            var type = fixedStorageLength != 0
                ? H5TAdapter.CreateFixedLengthStringType(fixedStorageLength)
                : H5TAdapter.CreateVariableLengthStringType();

            type.CharacterSet = cset;
            type.StringPadding = padding;

            return type;
        }

        return Create(h5Object, name, typeCtor, H5Space.CreateScalar, h => new H5StringAttribute(h));
    }

    internal static TA Open<T, TA>(H5Object<T> h5Object, string name, Func<long, TA> attributeCtor)
        where T : H5Object<T>
        where TA : H5Attribute
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

        return attributeCtor(h);
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

    internal static TT GetType<TT>(H5Attribute attribute, Func<long, TT> typeCtor) where TT : H5Type
    {
        return typeCtor(get_type(attribute));
    }

    #region Read

    internal static T Read<T>(H5PrimitiveAttribute<T> attribute, H5Type type) // where T : unmanaged
    {
        using var nativeType = H5Type.GetEquivalentNativeType<T>();
        return ReadImpl<T>(attribute, type, nativeType);
    }

    internal static T ReadImpl<T>(H5Attribute attribute, H5Type type, H5Type expectedType) //where T : unmanaged
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

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

        var cls = type.Class;

        var expectedCls = H5TAdapter.GetClass(expectedType);

        if (cls != expectedCls)
        {
            throw new H5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int attributeStorageSize = attribute.StorageSize;

        // We are relying on code consistency to ensure T is unmanaged since generic constraints aren't flexible enough

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        int marshalSize = sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, marshalSize);

        T value = default!;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        read(attribute, type, new IntPtr(&value)).ThrowIfError();
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        return value!;
    }

    #endregion

    #region Write

    internal static void Write<T>(H5Attribute attribute, H5Type type, T value) //where T : unmanaged
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        // We are relying on code consistency to ensure T is unmanaged since generic constraints aren't flexible enough

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        var size = sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

        int attributeStorageSize = attribute.StorageSize;
        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, size);

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        Write(attribute, type, new IntPtr(&value));
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    internal static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        write(attribute, type, buffer).ThrowIfError();
    }

    #endregion
}
