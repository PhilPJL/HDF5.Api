using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
using System.Collections.Generic;

namespace HDF5.Api;

public abstract class H5ObjectWithAttributes<T> : H5Object<T>, IH5ObjectWithAttributes where T : H5Object<T>
{
    internal H5ObjectWithAttributes(long handle, HandleType handleType, Action<T>? closeHandle)
        : base(handle, handleType, closeHandle)
    {
    }

    public int NumberOfAttributes => (int)H5OAdapter.GetInfo(this).num_attrs;

    public IEnumerable<string> AttributeNames => H5AAdapter.GetAttributeNames(this);

    /// <summary>
    ///     Create an Attribute for this location
    /// </summary>
    public H5Attribute CreateAttribute(
        [DisallowNull] string name,
        [DisallowNull] H5Type type,
        [DisallowNull] H5Space space)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5AAdapter.Create(this, name, type, space);
    }

/*    public H5Attribute CreateAttribute<TA>([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Create<T, TA>(this, name);
    }
*/
    /// <summary>
    /// Create and configure string attribute in the target location.
    /// </summary>
    /// <param name="name">Attribute name. This can be unicode and isn't impacted by the <paramref name="fixedStorageLength"/> value.</param>
    /// <param name="fixedStorageLength">The total number of bytes allocated to store the string including the null terminator.  
    /// <para>ASCII strings require 1 byte per character, plus 1 for the null terminator.</para>
    /// <para>UTF8 strings require 1-4 bytes per character, plus 1 for the null terminator.</para></param>
    /// <param name="characterSet">Defaults to <see cref="CharacterSet.Utf8"/>.</param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public H5Attribute CreateStringAttribute(
        [DisallowNull] string name,
        int fixedStorageLength = 0, CharacterSet characterSet = CharacterSet.Utf8, StringPadding padding = StringPadding.NullTerminate)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding);
    }

    /// <summary>
    ///     Open an existing Attribute for this location
    /// </summary>
    public H5Attribute OpenAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name);
    }

    public void DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        H5AAdapter.Delete(this, name);
    }

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public TA ReadAttribute<TA>([DisallowNull] string name) where TA : unmanaged, IEquatable<TA>
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.Read<TA>(attribute);
    }

    public TA ReadEnumAttribute<TA>([DisallowNull] string name, bool verifyType = false) where TA : unmanaged, Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadEnum<TA>(attribute, verifyType);
    }

    public string ReadStringAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadString(attribute);
    }

    public bool ReadBoolAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadBool(attribute);
    }

    public DateTime ReadDateTimeAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadDateTime(attribute);
    }

    public void Enumerate(Action<H5Attribute> action)
    {
        foreach (var name in AttributeNames)
        {
            using var h5Object = OpenAttribute(name);
            action(h5Object);
        }
    }

    public void CreateAndWriteAttribute<TA>([DisallowNull] string name, TA value)
        where TA : unmanaged, IEquatable<TA>
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var type = H5Type.GetEquivalentNativeType<TA>();

        if (typeof(TA) == typeof(bool))
        {
            var byteValue = (byte)(value.Equals(default) ? 0 : 0x01);
            CreateAndWriteAttribute(type, name, byteValue);
        }
        else
        {
            CreateAndWriteAttribute(type, name, value);
        }
    }

    public void CreateAndWriteEnumAttribute<TA>([DisallowNull] string name, TA value) where TA : unmanaged, Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var memorySpace = H5Space.CreateScalar();
        using var type = H5Type.CreateEnumType<TA>();
        using var attribute = CreateAttribute(name, type, memorySpace);
        H5AAdapter.Write(attribute, type, value);
    }

    private void CreateAndWriteAttribute<TA>(H5Type type, [DisallowNull] string name, TA value) where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var memorySpace = H5Space.CreateScalar();
        using var attribute = CreateAttribute(name, type, memorySpace);
        H5AAdapter.Write(attribute, type, value);
    }

    public void CreateAndWriteAttribute([DisallowNull] string name, DateTime value)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(name, value.ToOADate());
    }

    public void CreateAndWriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(value);

        using var attribute = CreateStringAttribute(name, fixedStorageLength, characterSet, padding);
        attribute.Write(value);
    }
}
