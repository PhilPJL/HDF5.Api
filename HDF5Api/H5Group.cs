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

    public static H5PropertyList CreatePropertyList(PropertyList propertyList)
    {
        return H5GAdapter.CreatePropertyList(propertyList);
    }

    /// <summary>
    /// Gets a copy of the specified property list used to create the object
    /// </summary>
    /// <param name="propertyList"></param>
    /// <returns></returns>
    public H5PropertyList GetPropertyList(PropertyList propertyList)
    {
        return H5GAdapter.GetPropertyList(this, propertyList);
    }
}
