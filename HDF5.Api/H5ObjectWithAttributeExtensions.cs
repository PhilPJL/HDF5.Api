using HDF5.Api.NativeMethodAdapters;
using CommunityToolkit.Diagnostics;
#if NETSTANDARD
using HDF5.Api.Disposables;
#endif
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif

namespace HDF5.Api;

public static class H5ObjectWithAttributeExtensions
{
    public static void CreateAndWriteAttribute<T>(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, T value,
        H5PropertyList? creationPropertyList = null) where T : unmanaged
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

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
        int maxLength = 0,
        H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNullOrWhiteSpace(value);

        value ??= string.Empty;

        maxLength = maxLength <= 0 ? value.Length : Math.Min(value.Length, maxLength);

        // TODO: variable length string

#pragma warning disable IDE0057 // Use range operator
        string subString = value.Length > maxLength ? value.Substring(0, maxLength) : value;
#pragma warning restore IDE0057 // Use range operator

        // can't create a zero length string type so use a length of 1 minimum
        using var type = H5TAdapter.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length);
        using var memorySpace = H5SAdapter.CreateScalar();
        using var attribute = owa.CreateAttribute(name, type, memorySpace, creationPropertyList);
        H5AAdapter.Write(attribute, subString);
    }
}
