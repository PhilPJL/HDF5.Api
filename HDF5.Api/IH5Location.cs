﻿using System.Collections.Generic;

namespace HDF5.Api;

/// <summary>
///     Operations that can be carried out on a location (group or file)
/// </summary>
/// <remarks>
///     Add more methods as required.
/// </remarks>
public interface IH5Location : IH5ObjectWithAttributes
{
    // Groups
    H5Group CreateGroup(string name,
        [AllowNull] H5PropertyList? linkCreationPropertyList = null,
        [AllowNull] H5PropertyList? groupCreationPropertyList = null,
        [AllowNull] H5PropertyList? groupAccessPropertyList = null);
    H5Group OpenGroup(string name);
    bool GroupExists(string name);
    bool GroupPathExists(string path);
    void DeleteGroup(string name);

    // Data sets
    H5DataSet CreateDataSet(string name, H5Type typeId, H5Space space, H5PropertyList propertyList);
    H5DataSet OpenDataSet(string name);
    bool DataSetExists(string name);
    // TODO: DataSetPathExists?
    // TODO: DeleteDataSet?

    IEnumerable<string> GroupNames { get; }
    IEnumerable<string> DataSetNames { get; }
    IEnumerable<string> NamedDataTypeNames { get; }
}