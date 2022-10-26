using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

namespace HDF5.Api;

public interface IH5ObjectWithAttributes
{
    // String
    void CreateAndWriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad);

    H5StringAttribute CreateStringAttribute(string name, int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8, StringPadding padding = StringPadding.NullTerminate);
    H5StringAttribute OpenStringAttribute([DisallowNull] string name);
    string ReadStringAttribute(string name);

    // Primitive
    void CreateAndWriteAttribute<TA>([DisallowNull] string name, TA value) where TA : unmanaged, IEquatable<TA>;
    H5PrimitiveAttribute<T> OpenPrimitiveAttribute<T>(string name) where T : unmanaged;
    T ReadPrimitiveAttribute<T>(string name) where T : unmanaged, IEquatable<T>;

    // Bool
    bool ReadBoolAttribute(string name);

    // Enum
    T ReadEnumAttribute<T>(string name) where T : unmanaged, Enum;

    // General
    void DeleteAttribute(string name);
    bool AttributeExists(string name);

    // Enumeration
    IEnumerable<string> AttributeNames { get; }
    void Enumerate(Action<H5Attribute> action); 
    int NumberOfAttributes { get; }
}

public static class H5ObjectWithAttributesExtensions
{
    public static DateTime ReadDateTimeAttribute([DisallowNull] this IH5ObjectWithAttributes h5Object, [DisallowNull] string name)
    {
        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);

        using var attribute = h5Object.OpenPrimitiveAttribute<double>(name);
        return DateTime.FromOADate(attribute.Read());
    }

    public static void CreateAndWriteDateTimeAttribute([DisallowNull] this IH5ObjectWithAttributes h5Object, 
        [DisallowNull] string name, DateTime value)
    {
        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);

        using var attribute = h5Object.OpenPrimitiveAttribute<double>(name);
        attribute.Write(value.ToOADate());
    }
}