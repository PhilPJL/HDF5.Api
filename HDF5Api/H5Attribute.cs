using HDF5Api.NativeMethodAdapters;
namespace HDF5Api;

/// <summary>
///     .NET class for the H5A (Attribute) API: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, H5AAdapter.Close)
    {
    }

    #region Public Api

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public H5Type GetH5Type()
    {
        return H5AAdapter.GetType(this);
    }

    public string ReadString()
    {
        using var type = GetH5Type();
        using var space = GetSpace();

        var count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();
        if (cls != H5Class.String)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting STRING.");
        }

        var size = H5AAdapter.GetStorageSize(this);

        Span<byte> buffer = stackalloc byte[(int)size];
        H5AAdapter.Read(this, type, buffer);
        return Encoding.ASCII.GetString(buffer);
    }

    public DateTime ReadDateTime()
    {
        return DateTime.FromOADate(Read<double>());
    }

    public T Read<T>() where T : unmanaged
    {
        using var type = GetH5Type();
        using var space = GetSpace();

        long count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();

        using var nativeType = H5Type.GetNativeType<T>();
        var expectedCls = H5TAdapter.GetClass(nativeType);

        if (cls != expectedCls)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int size = (int)H5AAdapter.GetStorageSize(this);

        if (size != Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
        }

        // TODO: consider memory alignment on different platforms 
        Span<T> buffer = stackalloc T[1];
        H5AAdapter.Read(this, type, buffer);
        return buffer[0];
    }

    // TODO: expose public Write<T> etc as per Read
    internal void Write(H5Type type, IntPtr buffer)
    {
        H5AAdapter.Write(this, type, buffer);
    }

    #endregion
}
