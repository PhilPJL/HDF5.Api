using HDF5.Api.Disposables;
using HDF5.Api.NativeMethodAdapters;
using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif

namespace HDF5.Api;

public static class H5ObjectWithAttributeExtensions
{
#if NETSTANDARD
    private static void CreateAndWriteAttribute(
        [DisallowNull] IH5ObjectWithAttributes owa, [DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] IntPtr buffer,
        H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);

        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SAdapter.CreateSimple(1);

        // TODO: variable length
        // TODO: create if not exist
        // TODO: throw if already exist

        // Create an attribute
        using var attribute = owa.CreateAttribute(name, type, memorySpace, creationPropertyList);
        H5AAdapter.Write(attribute, type, buffer);
    }
#endif

#if NET7_0_OR_GREATER
    private static void CreateAndWriteAttribute(
        [DisallowNull] IH5ObjectWithAttributes owa, [DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] Span<byte> buffer,
        H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);

        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SAdapter.CreateSimple(1);

        // TODO: variable length
        // TODO: create if not exist
        // TODO: throw if already exist

        // Create an attribute
        using var attribute = owa.CreateAttribute(name, type, memorySpace, creationPropertyList);
        H5AAdapter.Write(attribute, type, buffer);
    }
#endif

    public static void CreateAndWriteAttribute<T>(
        [DisallowNull] this IH5ObjectWithAttributes owa, [DisallowNull] string name, T value,
        H5PropertyList? creationPropertyList = null) where T : unmanaged
    {
        Guard.IsNotNull(owa);
        Guard.IsNotNullOrWhiteSpace(name);

        using var typeId = H5Type.GetNativeType<T>();

#if NETSTANDARD
        using var pinned = new PinnedObject(value);
        CreateAndWriteAttribute(owa, name, typeId, pinned, creationPropertyList);
#endif

#if NET7_0_OR_GREATER
        var size = Marshal.SizeOf<T>();

        if (size < 256)
        {
            Span<T> buffer = stackalloc T[1] { value };
            CreateAndWriteAttribute(owa, name, typeId, MemoryMarshal.Cast<T, byte>(buffer), creationPropertyList);
        }
        else
        {
            using var buffer = SpanOwner<T>.Allocate(size);
            buffer.Span[0] = value;
            CreateAndWriteAttribute(owa, name, typeId, MemoryMarshal.Cast<T, byte>(buffer.Span), creationPropertyList);
        }
#endif
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
        using var typeId = H5TAdapter.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length);

#if NETSTANDARD
        byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);      
        using var pinned = new PinnedObject(sourceBytes);
        CreateAndWriteAttribute(owa, name, typeId, pinned, creationPropertyList);
#endif

#if NET7_0_OR_GREATER
        // TODO: encoding UTF8?
        var span = H5TypeAdapterBase.Ascii.GetBytes(value).AsSpan();
        CreateAndWriteAttribute(owa, name, typeId, span, creationPropertyList);
#endif
    }

    internal static TV ReadAttribute<T, TV>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
        where TV : unmanaged
    {
        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.Read<TV>();
    }

    internal static string ReadStringAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.ReadString();
    }

    internal static DateTime ReadDateTimeAttribute<T>(this H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(h5Object, name);

        return attribute.ReadDateTime();
    }
}
