using HDF5.Api.NativeMethodAdapters;
using CommunityToolkit.Diagnostics;

namespace HDF5.Api;

public static class H5ObjectWithAttributeExtensions
{
    public static void CreateAndWriteAttribute<T>(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, T value,
        H5PropertyList? creationPropertyList = null) where T : unmanaged
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        // Add CreateScalarAttribute
        using var type = H5Type.GetNativeType<T>();
        using var memorySpace = H5SAdapter.CreateScalar();
        using var attribute = owa.CreateAttribute(name, type, memorySpace, creationPropertyList);
        H5AAdapter.Write(attribute, value);
    }

    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, DateTime value,
        H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(owa, name, value.ToOADate(), creationPropertyList);
    }

    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa,
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedLength,
        H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNullOrWhiteSpace(value);

        using var attribute = owa.CreateStringAttribute(name, fixedLength, creationPropertyList);

        if (value != string.Empty)
        {
            H5AAdapter.Write(attribute, value);
        }
    }
}
