using System.Collections.Generic;

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
    H5Group CreateGroup(string name);
    H5Group OpenGroup(string name);
    bool GroupExists(string name);
    bool GroupPathExists(string path);
    void DeleteGroup(string name);

    // Data sets
    H5DataSet CreateDataSet(string name, H5Type typeId, H5Space space,
        [AllowNull] H5DataSetCreationPropertyList? dataSetCreationPropertyList = null);
    H5DataSet OpenDataSet(string name);
    bool DataSetExists(string name);
    // TODO: DataSetPathExists?
    // TODO: DeleteDataSet?

    IEnumerable<string> GroupNames { get; }
    IEnumerable<string> DataSetNames { get; }
    IEnumerable<string> NamedDataTypeNames { get; }

    // TODO: move to IH5ObjectWithAttributes?
    IEnumerable<(string name, H5ObjectType type)> Members { get; }

    void Commit(
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type,
        [AllowNull] H5PropertyList? dataTypeCreationPropertyList = null,
        [AllowNull] H5PropertyList? dataTypeAccessPropertyList = null);

    string Name { get; }
}
