using CommunityToolkit.Diagnostics;
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.NativeMethods;
using System.Linq;
using System.Reflection;
using static HDF5.Api.NativeMethods.H5T;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_t.html"/>
/// </summary>
internal static unsafe class H5TAdapter
{
    internal static bool AreEqual(H5Type type1, H5Type type2)
    {
        return equal(type1, type2).ThrowIfError() > 0;
    }

    internal static void Close(H5Type type)
    {
        close(type).ThrowIfError();
    }

    internal static DataTypeClass GetClass(H5Type type)
    {
        return (DataTypeClass)get_class(type);
    }

    internal static void SetCharacterSet(H5Type type, CharacterSet cset)
    {
        set_cset(type, (cset_t)cset).ThrowIfError();
    }

    internal static CharacterSet GetCharacterSet(H5Type type)
    {
        return (CharacterSet)((int)get_cset(type)).ThrowIfError();
    }

    internal static void SetUTF8(H5Type type) => SetCharacterSet(type, CharacterSet.Utf8);
    internal static void SetAscii(H5Type type) => SetCharacterSet(type, CharacterSet.Ascii);

    internal static H5Type CreateCompoundType(int size)
    {
        return new H5Type(create((class_t)DataTypeClass.Compound, new ssize_t(size)));
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" />
    /// </summary>
    internal static H5Type CreateCompoundType<T>() where T : struct
    {
        return CreateCompoundType(Marshal.SizeOf<T>());
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    internal static H5Type CreateCompoundType<T>(int extraSpace) where T : struct
    {
        int size = Marshal.SizeOf<T>() + extraSpace;
        return CreateCompoundType(size);
    }

    internal static H5Type CreateByteArrayType(params long[] dims)
    {
        return new H5Type(array_create(NATIVE_B8, (uint)dims.Length, dims.Select(d => (ulong)d).ToArray()));
    }

    internal static H5Type CreateDoubleArrayType(int size)
    {
        return new H5Type(array_create(NATIVE_DOUBLE, 1, new[] { (ulong)size }));
    }

    internal static H5Type CreateFloatArrayType(int size)
    {
        return new H5Type(array_create(NATIVE_FLOAT, 1, new[] { (ulong)size }));
    }

    internal static H5Type CreateVariableLengthByteArrayType()
    {
        return new H5Type(vlen_create(NATIVE_B8));
    }

    internal static H5StringType CreateFixedLengthStringType(int storageLengthBytes)
    {
        long h = copy(C_S1);
        h.ThrowIfInvalidHandleValue();

        var type = new H5StringType(h);

        try
        {
            type.Size = storageLengthBytes;
        }
        catch
        {
            type.Dispose();
            throw;
        }

        return type;
    }

    internal static H5StringType CreateVariableLengthStringType()
    {
        return new H5StringType(create(class_t.STRING, VARIABLE));
    }

    internal static void InsertEnumMember<T>(H5Type type, string name, T value)
        where T : unmanaged, Enum
    {
#if NET7_0_OR_GREATER
        enum_insert(type, name, new IntPtr(&value)).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            enum_insert(type, nameBytesPtr, new IntPtr(&value)).ThrowIfError();
        }
#endif
    }

    internal static string NameOfEnumMember<T>(H5Type type, T value)
        where T : unmanaged, Enum
    {
        const int length = 512;

#if NET7_0_OR_GREATER
        using var bufferOwner = SpanOwner<byte>.Allocate(length);
        var buffer = bufferOwner.Span;
        enum_nameof(type, new nint(&value), buffer, length).ThrowIfError();
        int nullTerminatorIndex = MemoryExtensions.IndexOf(buffer, (byte)0);
        nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
        return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
#else
        var buffer = new byte[length];
        fixed (byte* bufferPtr = buffer)
        {
            enum_nameof(type, new IntPtr(&value), bufferPtr, new IntPtr(length)).ThrowIfError();
            Span<byte> bytes = buffer;
            var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
            nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
            return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
        }
#endif
    }

    internal static T ValueOfEnumMember<T>(H5Type type, string name)
        where T : unmanaged, Enum
    {
        T value = default;

#if NET7_0_OR_GREATER
        enum_valueof(type, name, new nint(&value)).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            enum_valueof(type, nameBytesPtr, new IntPtr(&value)).ThrowIfError();
        }
#endif

        return value;
    }

    internal static void Insert(H5Type type, string name, ssize_t offset, long nativeTypeId)
    {
#if NET7_0_OR_GREATER
        insert(type, name, offset, nativeTypeId).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            insert(type, nameBytesPtr, offset, nativeTypeId).ThrowIfError();
        }
#endif
    }

    internal static void Insert(H5Type typeId, string name, ssize_t offset, H5Type dataTypeId)
    {
        Insert(typeId, name, offset, (long)dataTypeId);
    }

    internal static bool IsVariableLengthString(H5Type typeId)
    {
        return is_variable_str(typeId).ThrowIfError() > 0;
    }

    internal static H5EnumType<T> CreateEnumType<T>() where T : unmanaged, Enum
    {
        var h5EnumType = CreateBaseEnumType<T>();

        try
        {
            foreach (var enumInfo in typeof(T)
                .GetMembers(BindingFlags.Public | BindingFlags.Static)
                .Select(m => new
                {
                    m.Name,
#if NET7_0_OR_GREATER
                    Value = Enum.Parse<T>(m.Name)
#else
                    Value = (T)Enum.Parse(typeof(T), m.Name)
#endif
                }))
            {
                InsertEnumMember(h5EnumType, enumInfo.Name, enumInfo.Value);
            }
        }
        catch
        {
            h5EnumType.Dispose();
            throw;
        }

        return h5EnumType;
    }

    internal static H5EnumType<T> CreateBaseEnumType<T>() where T : unmanaged, Enum
    {
        var underlyingType = Enum.GetUnderlyingType(typeof(T)).Name;

        long baseType = underlyingType switch
        {
            "Int64" => NATIVE_INT64,
            "UInt64" => NATIVE_UINT64,
            "Int32" => NATIVE_INT32,
            "UInt32" => NATIVE_UINT32,
            "Int16" => NATIVE_INT16,
            "UInt16" => NATIVE_UINT16,
            "Byte" => NATIVE_UINT8,
            _ => throw new ArgumentException($"Unable to create Enum for underlying type '{underlyingType}'."),
        };

        long h = enum_create(baseType);
        h.ThrowIfInvalidHandleValue();
        return new H5EnumType<T>(h);
    }

    internal static long GetNativeType<T>() where T : unmanaged
    {
        return default(T) switch
        {
            bool => NATIVE_HBOOL, // hmm bool has marshalable size of 4, but storage size of 1.

            byte => NATIVE_B8,
            sbyte => NATIVE_B8,

            short => NATIVE_INT16,
            ushort => NATIVE_USHORT,

            // TODO: check sizes 
            char => NATIVE_CHAR,

            int => NATIVE_INT32,
            uint => NATIVE_UINT32,

            long => NATIVE_INT64,
            ulong => NATIVE_UINT64,

            float => NATIVE_FLOAT,
            double => NATIVE_DOUBLE,

            // add more mappings as required

            _ => throw new H5Exception($"No mapping defined from {typeof(T).Name} to native type.")
        };
    }

    internal static void SetPadding(H5Type h5Type, StringPadding padding)
    {
        set_strpad(h5Type, (str_t)padding).ThrowIfError();
    }

    internal static StringPadding GetPadding(H5Type h5Type)
    {
        return (StringPadding)((int)get_strpad(h5Type)).ThrowIfError();
    }

    internal static void SetSize(H5Type h5Type, int size)
    {
        set_size(h5Type, new IntPtr(size)).ThrowIfError();
    }

    internal static int GetSize(H5Type h5Type)
    {
        return ((int)get_size(h5Type)).ThrowIfError();
    }

    internal static bool GetCommitted(H5Type h5Type)
    {
        return committed(h5Type).ThrowIfError() > 0;
    }

    internal static void Commit<T>(
        [DisallowNull] H5Object<T> h5Object,
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type,
        [AllowNull] H5DataTypeCreationPropertyList? dataTypeCreationPropertyList,
        [AllowNull] H5DataTypeAccessPropertyList? dataTypeAccessPropertyList) where T : H5Object<T>
    {
        h5Object.AssertHasLocationHandleType();

        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);
        Guard.IsNotNull(h5Type);

        using var linkCreationPropertyList = H5Link.CreateCreationPropertyList();

#if NET7_0_OR_GREATER
        commit(h5Object, name, h5Type,
            linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList)
            .ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            commit(h5Object, nameBytesPtr, h5Type,
                linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList)
                .ThrowIfError();
        }
#endif
    }

    internal static H5DataTypeCreationPropertyList CreateCreationPropertyList()
    {
        return H5PAdapter.Create(H5P.DATATYPE_CREATE, h => new H5DataTypeCreationPropertyList(h));
    }

    internal static H5DataTypeAccessPropertyList CreateAccessPropertyList()
    {
        return H5PAdapter.Create(H5P.DATATYPE_ACCESS, h => new H5DataTypeAccessPropertyList(h));
    }

    internal static void Lock(H5Type type)
    {
        lock_datatype(type).ThrowIfError();
    }

    internal static H5Type Open<T>(
        [DisallowNull] H5Object<T> h5Object,
        [DisallowNull] string name,
        [AllowNull] H5DataTypeAccessPropertyList? dataTypeAccessPropertyList) where T : H5Object<T>
    {
        h5Object.AssertHasLocationHandleType();

        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);

        long h;

#if NET7_0_OR_GREATER
        h = open(h5Object, name, dataTypeAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(h5Object, nameBytesPtr, dataTypeAccessPropertyList);
        }
#endif

        return new H5Type(h);
    }

    internal static int GetNumberOfMember(H5Type type)
    {
        return get_nmembers(type).ThrowIfError();
    }
}
