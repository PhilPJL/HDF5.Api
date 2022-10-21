using HDF5.Api.NativeMethodAdapters;
using CommunityToolkit.Diagnostics;

namespace HDF5.Api;

public static class H5ObjectWithAttributeExtensions
{
    public static void CreateAndWriteAttribute<T>(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, T value) where T : unmanaged
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        // Add CreateScalarAttribute
        using var type = H5Type.GetNativeType<T>();
        using var memorySpace = H5Space.CreateScalar();
        using var attribute = owa.CreateAttribute(name, type, memorySpace);
        H5AAdapter.Write(attribute, type, value);
    }

    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, DateTime value)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(owa, name, value.ToOADate());
    }

    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa,
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(value);

        using var attribute = owa.CreateStringAttribute(name, fixedStorageLength, characterSet, padding);

        H5AAdapter.WriteString(attribute, value);
    }
}
