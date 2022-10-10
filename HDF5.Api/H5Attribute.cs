using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

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
        return H5AAdapter.ReadString(this);
    }

    public T Read<T>() where T : unmanaged
    {
        return H5AAdapter.Read<T>(this);
    }

    public DateTime ReadDateTime()
    {
        return DateTime.FromOADate(Read<double>());
    }

    // TODO: expose public Write<T> etc as per Read
    internal void Write(H5Type type, IntPtr buffer)
    {
        H5AAdapter.Write(this, type, buffer);
    }

    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5AAdapter.GetPropertyList(this, listType);
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5AAdapter.CreatePropertyList(listType);
    }
}
