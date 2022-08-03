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

    H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId,
        H5PropertyListHandle propertyListId);
    H5DataSet OpenDataSet(string name);
    bool DataSetExists(string name);
    // TODO: DeleteDataSet by name?

    IEnumerable<(string name, bool isGroup)> GetChildNames();
}
