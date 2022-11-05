using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Attributes;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.Utils;
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

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public DeleteAttributeStatus DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        if (AttributeExists(name))
        {
            H5AAdapter.Delete(this, name);
            return DeleteAttributeStatus.Deleted;
        }

        return DeleteAttributeStatus.NotFound;
    }

    #region Open attribute 

    internal H5PrimitiveAttribute<TA> OpenPrimitiveAttribute<TA>(string name) //where TA : unmanaged
    {
        return H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h));
    }

    internal H5BooleanAttribute OpenBooleanAttribute(string name)
    {
        return H5AAdapter.Open(this, name, h => new H5BooleanAttribute(h));
    }

    public H5StringAttribute OpenStringAttribute(string name)
    {
        return H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
    }

    internal H5TimeSpanAttribute OpenTimeSpanAttribute(string name)
    {
        return H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h));
    }

#if NET7_0_OR_GREATER
    internal H5TimeOnlyAttribute OpenTimeOnlyAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5TimeOnlyAttribute(h));
    }
#endif

    internal H5DateTimeAttribute OpenDateTimeAttribute(string name)
    {
        return H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h));
    }

    internal H5DateTimeOffsetAttribute OpenDateTimeOffsetAttribute(string name)
    {
        return H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h));
    }

    public H5EnumAttribute<TA> OpenEnumAttribute<TA>([DisallowNull] string name) where TA : Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h));
    }

    #endregion

    public TA ReadAttribute<TA>([DisallowNull] string name, bool verifyType = false)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        bool exists = AttributeExists(name);

        if (!exists)
        {
            throw new H5Exception($"Attribute '{name}' does not exist.");
        }

        if (typeof(TA) == typeof(string))
        {
            return (TA)ReadStringAttribute();
        }

        return default(TA) switch
        {
            char or byte or sbyte or short or ushort or int or uint or long or ulong or float or double
                => (TA)ReadPrimitiveAttribute(),
            decimal => (TA)ReadDecimalAttribute(),
            bool => (TA)ReadBoolAttribute(),
            Enum => (TA)ReadEnumAttribute(),
            DateTime => (TA)ReadDateTimeAttribute(),
            DateTimeOffset => (TA)ReadDateTimeOffsetAttribute(),
            TimeSpan => (TA)ReadTimeSpanAttribute(),
#if NET7_0_OR_GREATER
            TimeOnly => (TA)ReadTimeOnlyAttribute(),
#endif
            _ => (TA)ReadCompoundAttribute()
        };

        object ReadPrimitiveAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h));
            return attribute.Read()!;
        }

        object ReadBoolAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5BooleanAttribute(h));
            return attribute.Read();
        }

        object ReadStringAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
            return attribute.Read();
        }

        object ReadEnumAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h));
            return attribute.Read(verifyType)!;
        }

        object ReadDateTimeAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h));
            return attribute.Read();
        }

#if NET7_0_OR_GREATER
        object ReadTimeOnlyAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5TimeOnlyAttribute(h));
            return attribute.Read();
        }
#endif

        object ReadDateTimeOffsetAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h));
            return attribute.Read();
        }

        object ReadTimeSpanAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h));
            return attribute.Read();
        }

        object ReadDecimalAttribute()
        {
            // TODO
            throw new NotImplementedException($"{typeof(TA).Name}");
        }

        object ReadCompoundAttribute()
        {
            // TODO
            if (typeof(TA).IsUnmanaged())
            {
                return ReadUnmanagedCompoundAttribute();
            }

            throw new NotImplementedException($"{typeof(TA).Name}");

            object ReadUnmanagedCompoundAttribute()
            {
                throw new NotImplementedException($"{typeof(TA).Name}");
            }
        }
    }

    #region Write attribute

    private bool CheckExists(string name, AttributeWriteBehaviour? writeBehaviour)
    {
        bool exists = AttributeExists(name);

        if (exists)
        {
            switch (writeBehaviour ?? H5Global.DefaultAttributeWriteBehaviour)
            {
                case AttributeWriteBehaviour.CreateOrUpdate: // Default
                    // do nothing - allow OpenAttribute to validate type
                    return true;
                case AttributeWriteBehaviour.OverwriteIfAlreadyExists:
                    DeleteAttribute(name);
                    return false;
                case AttributeWriteBehaviour.ThrowIfAlreadyExists:
                    throw new H5Exception($"Attribute '{name}' already exists.");
                default:
                    // Unknown value of AttributeWriteBehaviour
                    throw new InvalidEnumArgumentException(
                        nameof(H5Global.DefaultAttributeWriteBehaviour),
                        (int)H5Global.DefaultAttributeWriteBehaviour,
                        typeof(AttributeWriteBehaviour));
            }
        }

        return false;
    }

    public void WriteAttribute<TA>([DisallowNull] string name, [DisallowNull] TA value, AttributeWriteBehaviour? writeBehaviour = null) //where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(value);

        var exists = CheckExists(name, writeBehaviour);

        var shapeCtor = H5Space.CreateScalar; // TODO: arrays?

        if (typeof(TA).IsEnum)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5EnumAttribute<TA>(h))
                : H5AAdapter.Create(this, name, H5Type.CreateEnumType<TA>, shapeCtor, h => new H5EnumAttribute<TA>(h));
            
            attribute.Write(value);
        }
        else if (value is decimal decimalValue)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5DecimalAttribute(h))
                : H5AAdapter.Create(this, name, H5DecimalType.CreateType, shapeCtor, h => new H5DecimalAttribute(h));

            attribute.Write(decimalValue);
        }
        else if (value is DateTimeOffset dateTimeOffsetValue)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5DateTimeOffsetAttribute(h))
                : H5AAdapter.Create(this, name, H5DateTimeOffsetType.CreateType, shapeCtor, h => new H5DateTimeOffsetAttribute(h));

            attribute.Write(dateTimeOffsetValue);
        }
        else if (value is DateTime dateTimeValue)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5DateTimeAttribute(h))
                : H5AAdapter.Create(this, name, H5DateTimeType.CreateType, shapeCtor, h => new H5DateTimeAttribute(h));

            attribute.Write(dateTimeValue);
        }
        else if (value is TimeSpan timeSpanValue)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5TimeSpanAttribute(h))
                : H5AAdapter.Create(this, name, H5TimeSpanType.CreateType, shapeCtor, h => new H5TimeSpanAttribute(h));

            attribute.Write(timeSpanValue);
        }
#if NET7_0_OR_GREATER
        else if (value is TimeOnly timeOnlyValue)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5TimeOnlyAttribute(h))
                : H5AAdapter.Create(this, name, H5TimeOnlyType.CreateType, shapeCtor, h => new H5TimeOnlyAttribute(h));

            attribute.Write(timeOnlyValue);
        }
#endif
        else if (typeof(TA).IsPrimitive)
        {
            using var attribute = exists
                ? H5AAdapter.Open(this, name, h => new H5PrimitiveAttribute<TA>(h))
                : H5AAdapter.Create(this, name, H5Type.GetEquivalentNativeType<TA>, shapeCtor, h => new H5PrimitiveAttribute<TA>(h));

            attribute.Write(value);
        }
        else
        {
            // TODO: Compound
        }
    }

    /// <summary>
    /// Write string attribute.
    /// </summary>
    /// <param name="name">Attribute name. This can be unicode and isn't impacted by the <paramref name="fixedStorageLength"/> value.</param>
    /// <param name="fixedStorageLength">The total number of bytes allocated to store the string including the null terminator.  Set to 0 for a variable length string.
    /// <para>ASCII strings require 1 byte per character, plus 1 for the null terminator.</para>
    /// <para>UTF8 strings require 1-4 bytes per character, plus 1 for the null terminator.</para></param>
    /// <param name="characterSet">Defaults to <see cref="CharacterSet.Utf8"/>.</param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public void WriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] string value,
        int fixedStorageLength = 0,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(value);

        var exists = CheckExists(name, writeBehaviour);

        if (exists)
        {
            using var attribute = H5AAdapter.Open(this, name, h => new H5StringAttribute(h));
            attribute.Write(value);
        }
        else
        {
            using var attribute = H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding);
            attribute.Write(value);
        }
    }

    #endregion
}
