using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public H5Type GetH5Type()
    {
        return H5AAdapter.GetType(this);
    }

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    public int StorageSize => H5AAdapter.GetStorageSize(this);

    public string ReadString()
    {
        return H5AAdapter.ReadString(this);
    }

    public T Read<T>() where T : unmanaged
    {
        return H5AAdapter.Read<T>(this);
    }

    public DateTime ReadDateTime()
    {
        return H5AAdapter.ReadDateTime(this);
    }

    public void Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.WriteString(this, value);
    }

    public void Write<T>(T value) where T : unmanaged
    {
        H5AAdapter.Write(this, value);
    }

    public void Write(DateTime value)
    {
        H5AAdapter.Write(this, value);
    }

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public static H5Attribute CreateStringAttribute<T>(
        [DisallowNull] H5Object<T> h5Object,
        string name, int length = 0,
        CharacterSet characterSet = CharacterSet.Ascii, StringPadding padding = StringPadding.NullTerminate) where T : H5Object<T>
    {
        Guard.IsNotNull(h5Object);
        Guard.IsGreaterThanOrEqualTo(length, 0);
        
        h5Object.AssertHasWithAttributesHandleType();

        return H5AAdapter.CreateStringAttribute(h5Object, name, length, characterSet, padding);
    }
}
