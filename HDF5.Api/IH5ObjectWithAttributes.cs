using System.Collections.Generic;

namespace HDF5.Api;

public interface IH5ObjectWithAttributes
{
    // Create
    H5Attribute CreateAttribute(string name, H5Type typeId, H5Space space);
    void CreateAndWriteAttribute<TA>([DisallowNull] string name, TA value) where TA : unmanaged, IEquatable<TA>;
    void CreateAndWriteAttribute([DisallowNull] string name, DateTime value);
    void CreateAndWriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad);
    H5Attribute CreateStringAttribute(string name, int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8, StringPadding padding = StringPadding.NullTerminate);

    // Open
    H5Attribute OpenAttribute(string name);
    void DeleteAttribute(string name);
    bool AttributeExists(string name);

    // Read
    T ReadAttribute<T>(string name) where T : unmanaged, IEquatable<T>;
    string ReadStringAttribute(string name);
    DateTime ReadDateTimeAttribute(string name);
    bool ReadBoolAttribute(string name);

    // Enumeration
    IEnumerable<string> AttributeNames { get; }
    void Enumerate(Action<H5Attribute> action); 
    int NumberOfAttributes { get; }

}
