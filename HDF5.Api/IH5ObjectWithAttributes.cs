using System.Collections.Generic;

namespace HDF5.Api;

public interface IH5ObjectWithAttributes
{
    H5Attribute CreateAttribute(string name, H5Type typeId, H5Space space);
    H5Attribute OpenAttribute(string name);
    void DeleteAttribute(string name);
    bool AttributeExists(string name);

    T ReadAttribute<T>(string name) where T : unmanaged;
    string ReadStringAttribute(string name);
    DateTime ReadDateTimeAttribute(string name);

    IEnumerable<string> AttributeNames { get; }

    H5Attribute CreateStringAttribute(string name, int fixedStorageLength, 
        CharacterSet characterSet = CharacterSet.Ascii, StringPadding padding = StringPadding.NullTerminate);

    int NumberOfAttributes { get; }
}
