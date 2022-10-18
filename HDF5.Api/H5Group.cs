using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5G (Group) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_g.html"/>
/// </summary>
public class H5Group : H5Location<H5Group>
{
    internal H5Group(long handle) : base(handle, HandleType.Group, H5GAdapter.Close)
    {
    }

    public static H5GroupCreationPropertyList CreateCreationPropertyList()
    {
        return H5GAdapter.CreateCreationPropertyList();
    }

    /// <summary>
    /// Gets a copy of the specified property list used to create the object
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public H5GroupCreationPropertyList GetCreationPropertyList()
    {
        return H5GAdapter.GetCreationPropertyList(this);
    }

    public override string Name => H5IAdapter.GetName(this);
}
