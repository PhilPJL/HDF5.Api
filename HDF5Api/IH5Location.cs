using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Operations that can be carried out on a location (group or file)
/// </summary>
/// <remarks>
///     Add more methods as required.
/// </remarks>
public interface IH5Location : IH5ObjectWithAttributes
{
    H5Group CreateGroup(string name);
    H5Group OpenGroup(string name);
    bool GroupExists(string name);
    bool GroupPathExists(string path);
    void DeleteGroup(string name);

    H5DataSet CreateDataSet(string name, H5Type typeId, H5Space space, H5PropertyList propertyList);
    H5DataSet OpenDataSet(string name);
    bool DataSetExists(string name);
    // TODO: DeleteDataSet by name?

    IEnumerable<(string name, bool isGroup)> GetChildNames();
}
