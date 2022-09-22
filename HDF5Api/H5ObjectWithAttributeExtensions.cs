using System;
using HDF5Api.Disposables;

namespace HDF5Api;

internal static class H5ObjectWithAttributeExtensions
{
    private static void CreateAndWriteAttribute(IH5ObjectWithAttributes owa, string name, H5Type typeId, IntPtr buffer)
    {
        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SpaceNativeMethods.CreateSimple(1, new ulong[] { 1 }, new ulong[] { 1 });

        // Create the attribute-creation property list
        using var propertyList = H5PropertyListNativeMethods.Create(H5P.ATTRIBUTE_CREATE);

        // Create an attribute
        using var attribute = owa.CreateAttribute(name, typeId, memorySpace, propertyList);
        attribute.Write(typeId, buffer);
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

        if (maxLength <= 0)
        {
            maxLength = value.Length;
        }
        else
        {
            maxLength = Math.Min(value.Length, maxLength);
        }

        string subString = value.Length > maxLength ? value[..maxLength] : value;

        // can't create a zero length string type so use a length of 1 minimum
        using var typeId = H5Type.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length);
        byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);

        using var pinned = new PinnedObject(sourceBytes);
        CreateAndWriteAttribute(owa, name, typeId, pinned);
    }

    internal static T ReadAttribute<T>(this long handle, string name) where T : unmanaged
    {
        using var attribute = H5AttributeNativeMethods.Open(handle, name);

        return attribute.Read<T>();
    }

    internal static string ReadStringAttribute(this long handle, string name)
    {
        using var attribute = H5AttributeNativeMethods.Open(handle, name);

        return attribute.ReadString();
    }

    internal static DateTime ReadDateTimeAttribute(this long handle, string name)
    {
        using var attribute = H5AttributeNativeMethods.Open(handle, name);

        return attribute.ReadDateTime();
    }
}
