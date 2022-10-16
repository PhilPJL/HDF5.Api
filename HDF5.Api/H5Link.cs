using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

public class H5Link 
{
    /// <summary>
    /// Creates a <see cref="H5PropertyList"/> of the required type.
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5LAdapter.CreatePropertyList(listType);
    }
}

