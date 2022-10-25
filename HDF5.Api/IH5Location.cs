using System.Collections.Generic;

namespace HDF5.Api;

/// <summary>
///     Operations that can be carried out on a location (group or file)
/// </summary>
/// <remarks>
///     Add more methods as required.
/// </remarks>
public interface IH5Location
{
    // Groups
    H5Group CreateGroup(string name);
    H5Group OpenGroup(string name);
    bool GroupExists(string name);
    bool GroupPathExists(string path);
    void DeleteGroup(string name);
    IEnumerable<string> GroupNames { get; }
    void Enumerate(Action<H5Group> action);

    // Data sets
    H5DataSet CreateDataSet(string name, H5Type typeId, H5Space space,
        [AllowNull] H5DataSetCreationPropertyList? dataSetCreationPropertyList = null);
    H5DataSet OpenDataSet(string name);
    bool DataSetExists(string name);
    void DeleteDataSet(string name);
    IEnumerable<string> DataSetNames { get; }
    void Enumerate(Action<H5DataSet> action);


    // Data types
    IEnumerable<string> DataTypeNames { get; }
    H5Type OpenType(string name);
    void Enumerate(Action<H5Type> action);

    void Commit(
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type);

    // General
    IEnumerable<(string name, ObjectType type)> Members { get; }

    string Name { get; }
}
