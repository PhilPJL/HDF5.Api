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
        int err = equal(type1, type2);
        err.ThrowIfError();
        return err > 0;
    }

    internal static void Close(H5Type type)
    {
        int err = close(type);

        err.ThrowIfError();
    }

    internal static DataTypeClass GetClass(H5Type type)
    {
        return (DataTypeClass)get_class(type);
    }

    internal static void SetCharacterSet(H5Type type, CharacterSet cset)
    {
        int err = set_cset(type, (cset_t)cset);
        err.ThrowIfError();
    }

    internal static CharacterSet GetCharacterSet(H5Type type)
    {
        var cset = get_cset(type);
        ((int)cset).ThrowIfError();
        return (CharacterSet)cset;
    }

    internal static void SetUTF8(H5Type type) => SetCharacterSet(type, CharacterSet.Utf8);
    internal static void SetAscii(H5Type type) => SetCharacterSet(type, CharacterSet.Ascii);

    internal static H5Type CreateCompoundType(int size)
    {
        long h = create((class_t)DataTypeClass.Compound, new ssize_t(size));
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" />
    /// </summary>
    internal static H5Type CreateCompoundType<T>() where T : struct
    {
        int size = Marshal.SizeOf<T>();
        return CreateCompoundType(size);
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
        long h = array_create(NATIVE_B8, (uint)dims.Length, dims.Select(d => (ulong)d).ToArray());
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    internal static H5Type CreateDoubleArrayType(int size)
    {
        long h = array_create(NATIVE_DOUBLE, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    internal static H5Type CreateFloatArrayType(int size)
    {
        long h = array_create(NATIVE_FLOAT, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    internal static H5Type CreateVariableLengthByteArrayType()
    {
        long h = vlen_create(NATIVE_B8);
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    internal static H5StringType CreateFixedLengthStringType(int storageLengthBytes)
    {
        long h = copy(C_S1);
        h.ThrowIfInvalidHandleValue();
        int err = set_size(h, new IntPtr(storageLengthBytes));
        err.ThrowIfError();
        return new H5StringType(h);
    }

    internal static H5StringType CreateVariableLengthStringType()
    {
        long h = create(class_t.STRING, VARIABLE);
        h.ThrowIfInvalidHandleValue();
        return new H5StringType(h);
    }

    internal static void InsertEnumMember<T>(H5Type type, string name, T value)
        where T : unmanaged, Enum
    {
        int err;

#if NET7_0_OR_GREATER
        err = enum_insert(type, name, new IntPtr(&value));
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = enum_insert(type, nameBytesPtr, new IntPtr(&value));
        }
#endif

        err.ThrowIfError();
    }

    internal static string NameOfEnumMember<T>(H5Type type, T value)
        where T : unmanaged, Enum
    {
        const int length = 512;

#if NET7_0_OR_GREATER
        using var bufferOwner = SpanOwner<byte>.Allocate(length);
        var buffer = bufferOwner.Span;
        int err = enum_nameof(type, new nint(&value), buffer, length);
        err.ThrowIfError();
        int nullTerminatorIndex = MemoryExtensions.IndexOf(buffer, (byte)0);
        nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
        return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
#else
        var buffer = new byte[length];
        fixed (byte* bufferPtr = buffer)
        {
            int err = enum_nameof(type, new IntPtr(&value), bufferPtr, new IntPtr(length));
            err.ThrowIfError();
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

        int retval;

#if NET7_0_OR_GREATER
        retval = enum_valueof(type, name, new nint(&value));
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            retval = enum_valueof(type, nameBytesPtr, new IntPtr(&value));
        }
#endif
        retval.ThrowIfError();

        return value;
    }

    internal static void Insert(H5Type type, string name, ssize_t offset, long nativeTypeId)
    {
        int err;

#if NET7_0_OR_GREATER
        err = insert(type, name, offset, nativeTypeId);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = insert(type, nameBytesPtr, offset, nativeTypeId);
        }
#endif

        err.ThrowIfError();
    }

    internal static void Insert(H5Type typeId, string name, ssize_t offset, H5Type dataTypeId)
    {
        Insert(typeId, name, offset, (long)dataTypeId);
    }

    internal static bool IsVariableLengthString(H5Type typeId)
    {
        int err = is_variable_str(typeId);
        err.ThrowIfError();
        return err > 0;
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
        int err = set_strpad(h5Type, (str_t)padding);
        err.ThrowIfError();
    }

    internal static StringPadding GetPadding(H5Type h5Type)
    {
        var pad = get_strpad(h5Type);
        ((int)pad).ThrowIfError();
        return (StringPadding)pad;
    }

    internal static void SetSize(H5Type h5Type, int size)
    {
        int err = set_size(h5Type, new IntPtr(size));
        err.ThrowIfError();
    }

    internal static int GetSize(H5Type h5Type)
    {
        int size = (int)get_size(h5Type);
        size.ThrowIfError();
        return size;
    }

    internal static bool GetCommitted(H5Type h5Type)
    {
        int retval = committed(h5Type);
        retval.ThrowIfError();
        return retval > 0;
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

        int err;

#if NET7_0_OR_GREATER
        err = commit(h5Object, name, h5Type,
            linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = commit(h5Object, nameBytesPtr, h5Type,
                linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList);
        }
#endif

        err.ThrowIfError();
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
        int err = lock_datatype(type);
        err.ThrowIfError();
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

        h.ThrowIfInvalidHandleValue();

        return new H5Type(h);
    }
}
