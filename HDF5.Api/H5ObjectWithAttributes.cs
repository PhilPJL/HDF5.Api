using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Attributes;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using System.Collections.Generic;

namespace HDF5.Api;

public abstract class H5ObjectWithAttributes<T> : H5Object<T> where T : H5ObjectWithAttributes<T>
{
    internal H5ObjectWithAttributes(long handle, HandleType handleType, Action<T> closeHandle)
        : base(handle, handleType, closeHandle)
    {
    }

    public int NumberOfAttributes => (int)H5OAdapter.GetInfo(this).num_attrs;

    public IEnumerable<string> AttributeNames => H5AAdapter.GetAttributeNames(this);

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
    public H5StringAttribute CreateStringAttribute(
        [DisallowNull] string name,
        int fixedStorageLength = 0, CharacterSet characterSet = CharacterSet.Utf8, StringPadding padding = StringPadding.NullTerminate)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding);
    }

    /// <summary>
    ///     Open an existing Attribute for this location
    /// </summary>
    public H5PrimitiveAttribute<TA> OpenPrimitiveAttribute<TA>([DisallowNull] string name) where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h));
    }

    public H5BooleanAttribute OpenBooleanAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5BooleanAttribute(h));
    }

    public H5StringAttribute OpenStringAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
    }

    public H5TimeSpanAttribute OpenTimeSpanAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h));
    }

#if NET7_0_OR_GREATER
    public H5TimeOnlyAttribute OpenTimeOnlyAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5TimeOnlyAttribute(h));
    }
#endif

    public H5DateTimeAttribute OpenDateTimeAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h));
    }

    public H5DateTimeOffsetAttribute OpenDateTimeOffsetAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h));
    }

    public H5EnumAttribute<TA> OpenEnumAttribute<TA>([DisallowNull] string name) where TA : Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h));
    }

    public void DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        if (AttributeExists(name))
        {
            H5AAdapter.Delete(this, name);
        }

        // TODO: return status (not found, deleted)
    }

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public TA ReadAttribute<TA>([DisallowNull] string name, bool verifyType = false)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        if (typeof(TA) == typeof(string))
        {
            return (TA)ReadStringAttribute(name);
        }

        return default(TA) switch
        {
            char or byte or sbyte or short or ushort or int or uint or long or ulong or float or double
                => (TA)ReadPrimitiveAttribute(name),
            decimal => throw new NotImplementedException(),
            bool => (TA)ReadBoolAttribute(name),
            Enum => (TA)ReadEnumAttribute(name, verifyType),
            DateTime => (TA)ReadDateTimeAttribute(name),
            DateTimeOffset => (TA)ReadDateTimeOffsetAttribute(name),
            TimeSpan => (TA)ReadTimeSpanAttribute(name),
#if NET7_0_OR_GREATER
            TimeOnly => (TA)ReadTimeOnlyAttribute(name),
#endif
            _ => throw new NotImplementedException($"{typeof(TA).Name}")
        };

        object ReadPrimitiveAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h));
            return attribute.Read()!;
        }

        object ReadBoolAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5BooleanAttribute(h));
            return attribute.Read();
        }

        object ReadStringAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
            return attribute.Read();
        }

        object ReadEnumAttribute(string name, bool verifyType = false) 
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h));
            return attribute.Read(verifyType)!;
        }

        object ReadDateTimeAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h));
            return attribute.Read();
        }

#if NET7_0_OR_GREATER
        object ReadTimeOnlyAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5TimeOnlyAttribute(h));
            return attribute.Read();
        }
#endif

        object ReadDateTimeOffsetAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h));
            return attribute.Read();
        }

        object ReadTimeSpanAttribute(string name)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h));
            return attribute.Read();
        }
    }

/*    public TA ReadEnumAttribute<TA>([DisallowNull] string name, bool verifyType = false) where TA : unmanaged, Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h));
        return attribute.Read(verifyType);
    }
*/
    // These methods would be faster - no (or less) boxing
    /*    public TA ReadPrimitiveAttribute<TA>([DisallowNull] string name) where TA : unmanaged, IEquatable<TA>
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h));
            return attribute.Read()!;
        }

        public string ReadStringAttribute([DisallowNull] string name)
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
            return attribute.Read();
        }

        public bool ReadBoolAttribute([DisallowNull] string name)
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5BooleanAttribute(h));
            return attribute.Read();
        }

        public DateTime ReadDateTimeAttribute([DisallowNull] string name)
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h));
            return attribute.Read();
        }

        public DateTimeOffset ReadDateTimeOffsetAttribute([DisallowNull] string name)
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h));
            return attribute.Read();
        }

        public TimeSpan ReadTimeSpanAttribute([DisallowNull] string name)
        {
            Guard.IsNotNullOrWhiteSpace(name);

            using var attribute = H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h));
            return attribute.Read();
        }
    */
    // TODO
    /*    public void Enumerate(Action<H5Attribute> action)
        {
            foreach (var name in AttributeNames)
            {
                using var h5Object = H5AAdapter.Open()...;
                action(h5Object);
            }
        }*/


    public void CreateAndWriteAttribute<TA>([DisallowNull] string name, TA value)
        where TA : unmanaged, IEquatable<TA>
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var type = H5Type.GetEquivalentNativeType<TA>();

        CreateAndWriteAttribute(type, name, value);
    }

    public void CreateAndWriteEnumAttribute<TA>([DisallowNull] string name, TA value) where TA : unmanaged, Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var type = H5Type.CreateEnumType<TA>();
        using var space = H5Space.CreateScalar();
        using var attribute = H5AAdapter.Create(this, name, type, space, h => new H5EnumAttribute<TA>(h));

        attribute.Write(value);
    }

    private void CreateAndWriteAttribute<TA>(H5Type type, [DisallowNull] string name, TA value) where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var space = H5Space.CreateScalar();
        using var attribute = H5AAdapter.Create(this, name, type, space, h => new H5PrimitiveAttribute<TA>(h));

        attribute.Write(value);
    }

    public void CreateAndWriteAttribute([DisallowNull] string name, DateTimeOffset value)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var type = H5TAdapter.CreateDateTimeOffsetType();
        using var space = H5Space.CreateScalar();
        using var attribute = H5AAdapter.Create(this, name, type, space, h => new H5DateTimeOffsetAttribute(h));

        attribute.Write(value);
    }

    public void CreateAndWriteAttribute([DisallowNull] string name, DateTime value)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(name, value.ToBinary());
    }

    public void CreateAndWriteAttribute([DisallowNull] string name, TimeSpan value)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(name, value.Ticks);
    }

#if NET7_0_OR_GREATER
    public void CreateAndWriteAttribute([DisallowNull] string name, TimeOnly value)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        CreateAndWriteAttribute(name, value.Ticks);
    }
#endif

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
