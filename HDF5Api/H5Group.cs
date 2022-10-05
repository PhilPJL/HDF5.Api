using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5G (Group) API.
/// </summary>
public class H5Group : H5Location<H5Group>
{
    internal H5Group(long handle) : base(handle, H5GAdapter.Close)
    {
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5GAdapter.CreatePropertyList(listType);
    }

    /// <summary>
    /// Gets a copy of the specified property list used to create the object
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5GAdapter.GetPropertyList(this, listType);
    }
}
