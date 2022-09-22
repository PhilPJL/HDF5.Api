using System;
using System.Collections.Generic;

namespace HDF5Api;

public interface IH5ObjectWithAttributes
{
    H5Attribute CreateAttribute(string name, H5Type typeId, H5Space space, H5PropertyList propertyList);
    H5Attribute OpenAttribute(string name);
    void DeleteAttribute(string name);
    bool AttributeExists(string name);
    T ReadAttribute<T>(string name) where T : unmanaged;
    string ReadStringAttribute(string name);
    DateTime ReadDateTimeAttribute(string name);
    IEnumerable<string> ListAttributeNames();
}
