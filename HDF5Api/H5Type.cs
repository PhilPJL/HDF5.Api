using CommunityToolkit.Diagnostics;
using HDF5Api.NativeMethodAdapters;
using HDF5Api.NativeMethods;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5T (Type) API.
/// </summary>
public class H5Type : H5Object<H5Type>
{
    internal H5Type(long handle) : base(handle, H5TAdapter.Close)
    {
    }

    private H5Type(long handle, Action<H5Type>? closer) : base(handle, closer) { }

    #region Public Api

    public static H5Type GetNativeType<T>() where T : unmanaged
    {
        long nativeHandle = default(T) switch
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

        return new H5Type(nativeHandle, null);
    }

    public H5Type Insert([DisallowNull] string name, int offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, new IntPtr(offset), dataType);
        return this;
    }

    public H5Type Insert([DisallowNull] string name, IntPtr offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, offset, dataType);
        return this;
    }

    public H5Type Insert<S>([DisallowNull] string name, [DisallowNull] H5Type dataType) where S : struct
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        var offset = Marshal.OffsetOf<S>(name);
        H5TAdapter.Insert(this, name, offset, dataType);
        return this;
    }

    public H5Class GetClass()
    {
        return H5TAdapter.GetClass(this);
    }

    public static H5Type CreateDoubleArrayType(int size)
    {
        return H5TAdapter.CreateDoubleArrayType(size);
    }

    #endregion
}
