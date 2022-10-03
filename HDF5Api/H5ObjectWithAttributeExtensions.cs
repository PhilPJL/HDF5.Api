﻿using HDF5Api.Disposables;

using HDF5Api.NativeMethods;
using HDF5Api.NativeMethodAdapters;
using CommunityToolkit.Diagnostics;

namespace HDF5Api;

public static class H5ObjectWithAttributeExtensions
{
    private static void CreateAndWriteAttribute(
        [DisallowNull] IH5ObjectWithAttributes owa, [DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] IntPtr buffer)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);

        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SAdapter.CreateSimple(1);

        // Create the attribute-creation property list (TODO: is this required or can I use default?)
        using var propertyList = H5PAdapter.Create(H5P.ATTRIBUTE_CREATE);

        // Create an attribute
        using var attribute = owa.CreateAttribute(name, type, memorySpace, propertyList);
        attribute.Write(type, buffer);
    }

    public static void CreateAndWriteAttribute<T>(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, T value) where T : unmanaged
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        using var typeId = H5Type.GetNativeType<T>();
        using var pinned = new PinnedObject(value);
        CreateAndWriteAttribute(owa, name, typeId, pinned);
    }

    // Specific implementation for DateTime
    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, DateTime value)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(owa, name, value.ToOADate());
    }

    // Specific implementation for string
    public static void CreateAndWriteAttribute(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, [DisallowNull] string value, int maxLength = 0)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNullOrWhiteSpace(value);

        value ??= string.Empty;

        maxLength = maxLength <= 0 ? value.Length : Math.Min(value.Length, maxLength);
        
#pragma warning disable IDE0057 // Use range operator
        string subString = value.Length > maxLength ? value.Substring(0, maxLength) : value;
#pragma warning restore IDE0057 // Use range operator

        // can't create a zero length string type so use a length of 1 minimum
        using var typeId = H5TAdapter.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length);
        byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);

        using var pinned = new PinnedObject(sourceBytes);
        CreateAndWriteAttribute(owa, name, typeId, pinned);
    }

    internal static TV ReadAttribute<T, TV>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
        where TV : unmanaged
    {
        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.Read<TV>();
    }

    internal static string ReadStringAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.ReadString();
    }

    internal static DateTime ReadDateTimeAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.ReadDateTime();
    }
}
