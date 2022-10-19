using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_l.html"/>
/// </summary>
internal static class H5Link 
{
    /// <summary>
    /// Creates a <see cref="H5LinkCreationPropertyList"/> of the required type.
    /// </summary>
    /// <returns></returns>
    internal static H5LinkCreationPropertyList CreateCreationPropertyList(
        CharacterSet encoding = CharacterSet.Utf8, bool createIntermediateGroups = true)
    {
        return H5LAdapter.CreateCreationPropertyList(encoding, createIntermediateGroups);
    }
}

