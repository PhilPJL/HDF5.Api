using System;
using System.Collections.Generic;

namespace HDF5Api;

public interface IH5ObjectWithAttributes
{
    H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
    H5Attribute OpenAttribute(string name);
    void DeleteAttribute(string name);
    bool AttributeExists(string name);
    T ReadAttribute<T>(string name) where T : unmanaged;
    string ReadStringAttribute(string name);
    DateTime ReadDateTimeAttribute(string name);
    IEnumerable<string> ListAttributeNames();
}
