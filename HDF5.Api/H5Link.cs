﻿using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_l.html"/>
/// </summary>
public class H5Link 
{
    /// <summary>
    /// Creates a <see cref="H5PropertyList"/> of the required type.
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    internal static H5LinkCreationPropertyList CreateCreationPropertyList(
        CharacterSet encoding = CharacterSet.Utf8, bool createIntermediateGroups = true)
    {
        return H5LAdapter.CreateCreationPropertyList(encoding, createIntermediateGroups);
    }
}
