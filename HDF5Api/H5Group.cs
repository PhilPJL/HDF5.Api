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
}
