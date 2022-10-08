using HDF5Api.NativeMethods;
using System.Linq;
using static HDF5Api.NativeMethods.H5T;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5TAdapter
{
    public static void Close(H5Type type)
    {
        int err = close(type);

        err.ThrowIfError();
    }

    public static H5Class GetClass(H5Type typeId)
    {
        return (H5Class)get_class(typeId);
    }

    public static H5Type CreateCompoundType(int size)
    {
        long h = create((class_t)H5Class.Compound, new ssize_t(size));
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" />
    /// </summary>
    public static H5Type CreateCompoundType<T>() where T : struct
    {
        int size = Marshal.SizeOf<T>();
        return CreateCompoundType(size);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    public static H5Type CreateCompoundType<T>(int extraSpace) where T : struct
    {
        int size = Marshal.SizeOf<T>() + extraSpace;
        return CreateCompoundType(size);
    }

    public static H5Type CreateByteArrayType(params long[] dims)
    {
        long h = array_create(NATIVE_B8, (uint)dims.Length, dims.Select(d => (ulong)d).ToArray());
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    public static H5Type CreateDoubleArrayType(int size)
    {
        long h = array_create(NATIVE_DOUBLE, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    public static H5Type CreateFloatArrayType(int size)
    {
        long h = array_create(NATIVE_FLOAT, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    public static H5Type CreateVariableLengthByteArrayType()
    {
        long h = vlen_create(NATIVE_B8);
        h.ThrowIfInvalidHandleValue();
        return new H5Type(h);
    }

    public static H5Type CreateFixedLengthStringType(int length)
    {
        long h = copy(C_S1);
        h.ThrowIfInvalidHandleValue();
        int err = set_size(h, new ssize_t(length));
        err.ThrowIfError();
        return new H5Type(h);
    }

    public static void Insert(H5Type typeId, string name, ssize_t offset, long nativeTypeId)
    {
        int err = insert(typeId, name, offset, nativeTypeId);
        err.ThrowIfError();
    }

    public static void Insert(H5Type typeId, string name, ssize_t offset, H5Type dataTypeId)
    {
        int err = insert(typeId, name, offset, dataTypeId);
        err.ThrowIfError();
    }

    public static bool IsVariableLengthString(H5Type typeId)
    {
        int err = is_variable_str(typeId);
        err.ThrowIfError();
        return err > 0;
    }

    public static long GetNativeType<T>() where T : unmanaged
    {
        return default(T) switch
        {
            //            char => H5T.NATIVE_CHAR,

            short => H5T.NATIVE_INT16,
            ushort => H5T.NATIVE_USHORT,
            int => H5T.NATIVE_INT32,
            uint => H5T.NATIVE_UINT32,
            long => H5T.NATIVE_INT64,
            ulong => H5T.NATIVE_UINT64,
            float => H5T.NATIVE_FLOAT,
            double => H5T.NATIVE_DOUBLE,
            // TODO: add more mappings as required

            _ => throw new Hdf5Exception($"No mapping defined from {typeof(T).Name} to native type.")
        };       
    }
}
