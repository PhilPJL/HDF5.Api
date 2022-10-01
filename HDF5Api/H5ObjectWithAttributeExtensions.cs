using HDF5Api.Disposables;

using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

public static class H5ObjectWithAttributeExtensions
{
    private static void CreateAndWriteAttribute(IH5ObjectWithAttributes owa, string name, H5Type type, IntPtr buffer)
    {
        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SpaceNativeMethods.CreateSimple(new Dimension(1));

        // Create the attribute-creation property list
        using var propertyList = H5PropertyListNativeMethods.Create(NativeMethods.H5P.ATTRIBUTE_CREATE);

        // Create an attribute
        using var attribute = owa.CreateAttribute(name, type, memorySpace, propertyList);
        attribute.Write(type, buffer);
    }

    public static void CreateAndWriteAttribute<T>(this IH5ObjectWithAttributes owa, string name, T value) where T : unmanaged
    {
        using var typeId = H5Type.GetNativeType<T>();
        using var pinned = new PinnedObject(value);
        CreateAndWriteAttribute(owa, name, typeId, pinned);
    }

    // Specific implementation for DateTime
    public static void CreateAndWriteAttribute(this IH5ObjectWithAttributes owa, string name, DateTime value)
    {
        CreateAndWriteAttribute(owa, name, value.ToOADate());
    }

    // Specific implementation for string
    public static void CreateAndWriteAttribute(this IH5ObjectWithAttributes owa, string name, string value, int maxLength = 0)
    {
        value ??= string.Empty;

        maxLength = maxLength <= 0 ? value.Length : Math.Min(value.Length, maxLength);

        string subString = value.Length > maxLength ? value[..maxLength] : value;

        // can't create a zero length string type so use a length of 1 minimum
        using var typeId = H5TypeNativeMethods.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length);
        byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);

        using var pinned = new PinnedObject(sourceBytes);
        CreateAndWriteAttribute(owa, name, typeId, pinned);
    }

    internal static TV ReadAttribute<T, TV>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
        where TV : unmanaged
    {
        using var attribute = H5A.Open(h5Object, name);

        return attribute.Read<TV>();
    }

    internal static string ReadStringAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        using var attribute = H5A.Open(h5Object, name);

        return attribute.ReadString();
    }

    internal static DateTime ReadDateTimeAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        using var attribute = H5A.Open(h5Object, name);

        return attribute.ReadDateTime();
    }
}
