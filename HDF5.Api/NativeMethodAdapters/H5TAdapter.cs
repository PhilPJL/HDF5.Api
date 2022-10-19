using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethods;
using System.Linq;
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

    internal static H5Class GetClass(H5Type type)
    {
        return (H5Class)get_class(type);
    }

    internal static void SetCharacterSet(H5Type type, CharacterSet cset)
    {
        int err = set_cset(type, (cset_t)cset);
        err.ThrowIfError();
    }

    internal static CharacterSet GetCharacterSet(H5Type type)
    {
        cset_t cset = get_cset(type);
        ((int)cset).ThrowIfError();
        return (CharacterSet)cset;
    }

    internal static void SetUTF8(H5Type type) => SetCharacterSet(type, CharacterSet.Utf8);
    internal static void SetAscii(H5Type type) => SetCharacterSet(type, CharacterSet.Ascii);

    internal static H5Type CreateCompoundType(int size)
    {
        long h = create((class_t)H5Class.Compound, new ssize_t(size));
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

    internal static H5Type CreateFixedLengthStringType(int storageLengthBytes)
    {
        long h = copy(C_S1);
        h.ThrowIfInvalidHandleValue();
        int err = set_size(h, new IntPtr(storageLengthBytes));
        err.ThrowIfError();
        return new H5Type(h);
    }

    internal static H5Type CreateVariableLengthStringType()
    {
        long h = create(class_t.STRING, VARIABLE);
        return new H5Type(h);
    }

    internal static void Insert(H5Type typeId, string name, ssize_t offset, long nativeTypeId)
    {
        int err;

#if NET7_0_OR_GREATER
        err = insert(typeId, name, offset, nativeTypeId);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = insert(typeId, nameBytesPtr, offset, nativeTypeId);
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

    internal static long GetNativeType<T>() where T : unmanaged
    {
        return default(T) switch
        {
            //bool => NATIVE_HBOOL, hmm bool has marshalable size of 4, but storage size of 1.
            byte => NATIVE_B8,

            short => NATIVE_INT16,
            int => NATIVE_INT32,
            long => NATIVE_INT64,

            ushort => NATIVE_USHORT,
            uint => NATIVE_UINT32,
            ulong => NATIVE_UINT64,

            float => NATIVE_FLOAT,
            double => NATIVE_DOUBLE,
            // add more mappings as required

            _ => throw new Hdf5Exception($"No mapping defined from {typeof(T).Name} to native type.")
        }; ;
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
        int retval = (int)committed(h5Type);
        retval.ThrowIfError();
        return retval > 0;
    }

    internal static void Commit<T>(
        [DisallowNull] H5Object<T> h5Object,
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type,
        [AllowNull] H5PropertyList? dataTypeCreationPropertyList = null,
        [AllowNull] H5PropertyList? dataTypeAccessPropertyList = null) where T : H5Object<T>
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
}
