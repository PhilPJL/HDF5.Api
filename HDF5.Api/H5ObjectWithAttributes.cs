using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Attributes;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HDF5.Api;

public abstract class H5ObjectWithAttributes<T> : H5Object<T> where T : H5ObjectWithAttributes<T>
{
    internal H5ObjectWithAttributes(long handle, HandleType handleType, Action<T> closeHandle)
        : base(handle, handleType, closeHandle)
    {
    }

    public virtual int NumberOfAttributes => (int)H5OAdapter.GetInfo(this).num_attrs;

    public virtual IEnumerable<string> AttributeNames => H5AAdapter.GetAttributeNames(this);

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public virtual DeleteAttributeStatus DeleteAttribute([DisallowNull] string name)
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
        return H5AAdapter.Open(this, name, static h => new H5PrimitiveAttribute<TA>(h));
    }

    internal H5BooleanAttribute OpenBooleanAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5BooleanAttribute(h));
    }

    internal H5DecimalAttribute OpenDecimalAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5DecimalAttribute(h));
    }

    internal H5StringAttribute OpenStringAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5StringAttribute(h));
    }

    internal H5TimeSpanAttribute OpenTimeSpanAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5TimeSpanAttribute(h));
    }

#if NET7_0_OR_GREATER
    internal H5TimeOnlyAttribute OpenTimeOnlyAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, static h => new H5TimeOnlyAttribute(h));
    }

    internal H5DateOnlyAttribute OpenDateOnlyAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, static h => new H5DateOnlyAttribute(h));
    }
#endif

    internal H5DateTimeAttribute OpenDateTimeAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5DateTimeAttribute(h));
    }

    internal H5DateTimeOffsetAttribute OpenDateTimeOffsetAttribute(string name)
    {
        return H5AAdapter.Open(this, name, static h => new H5DateTimeOffsetAttribute(h));
    }

    internal H5EnumAttribute<TA> OpenEnumAttribute<TA>([DisallowNull] string name) where TA : Enum
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name, static h => new H5EnumAttribute<TA>(h));
    }

    #endregion

    #region Read attribute

    public TA ReadAttribute<TA>([DisallowNull] string name)
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
            DateOnly => (TA)ReadDateOnlyAttribute(),
#endif
            _ => (TA)ReadCompoundAttribute()
        };

        object ReadPrimitiveAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5PrimitiveAttribute<TA>(h));
            return attribute.Read()!;
        }

        object ReadBoolAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5BooleanAttribute(h));
            return attribute.Read();
        }

        object ReadStringAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5StringAttribute(h));
            return attribute.Read();
        }

        object ReadEnumAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5EnumAttribute<TA>(h));
            return attribute.Read()!;
        }

        object ReadDateTimeAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5DateTimeAttribute(h));
            return attribute.Read();
        }

#if NET7_0_OR_GREATER
        object ReadTimeOnlyAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5TimeOnlyAttribute(h));
            return attribute.Read();
        }

        object ReadDateOnlyAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5DateOnlyAttribute(h));
            return attribute.Read();
        }
#endif

        object ReadDateTimeOffsetAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5DateTimeOffsetAttribute(h));
            return attribute.Read();
        }

        object ReadTimeSpanAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5TimeSpanAttribute(h));
            return attribute.Read();
        }

        object ReadDecimalAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5DecimalAttribute(h));
            return attribute.Read();
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

    public IEnumerable<TA> ReadAttribute<TA>([DisallowNull] string name, long dimension1, params long[] dimensions)
    {
        return default(TA) switch
        {
            char or byte or sbyte or short or ushort or int or uint or long or ulong or float or double
                => ReadPrimitiveAttribute(),
            _ => Array.Empty<TA>()
        };

        IEnumerable<TA> ReadPrimitiveAttribute()
        {
            using var attribute = H5AAdapter.Open(this, name, static h => new H5PrimitiveAttribute<TA>(h));
            return attribute.ReadCollection()!;
        }
    }

    #endregion

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
        var shapeCtor = H5Space.CreateScalar;

        var type = typeof(TA);

        if (value is IEnumerable<string> values)
        {
            WriteAttribute(name, values, 0, writeBehaviour: writeBehaviour);
        }
        else if (value is string stringValue)
        {
            // Write a variable length string with default encoding and padding
            WriteAttribute(name, stringValue, 0, writeBehaviour: writeBehaviour);
        }
        else if (value is Array array)
        {
            WriteAttribute(name, array, 0, writeBehaviour: writeBehaviour);
        }
        else if (type.IsEnum)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5EnumType<TA>.Create, shapeCtor, static h => new H5EnumAttribute<TA>(h));

            attribute.Write(value);
        }
        else if (value is bool boolValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5BooleanType.Create, shapeCtor, H5BooleanAttribute.Create);

            attribute.Write(boolValue);
        }
        else if (value is decimal decimalValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DecimalType.Create, shapeCtor, H5DecimalAttribute.Create);

            attribute.Write(decimalValue);
        }
        else if (value is DateTimeOffset dateTimeOffsetValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateTimeOffsetType.Create, shapeCtor, H5DateTimeOffsetAttribute.Create);

            attribute.Write(dateTimeOffsetValue);
        }
        else if (value is DateTime dateTimeValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateTimeType.Create, shapeCtor, H5DateTimeAttribute.Create);

            attribute.Write(dateTimeValue);
        }
        else if (value is TimeSpan timeSpanValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5TimeSpanType.Create, shapeCtor, H5TimeSpanAttribute.Create);

            attribute.Write(timeSpanValue);
        }
#if NET7_0_OR_GREATER
        else if (value is TimeOnly timeOnlyValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5TimeOnlyType.Create, shapeCtor, H5TimeOnlyAttribute.Create);

            attribute.Write(timeOnlyValue);
        }
        else if (value is DateOnly dateOnlyValue)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateOnlyType.Create, shapeCtor, H5DateOnlyAttribute.Create);

            attribute.Write(dateOnlyValue);
        }
#endif
        else if (type.IsPrimitive)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5PrimitiveType<TA>.Create, shapeCtor, H5PrimitiveAttribute<TA>.Create);

            attribute.Write(value);
        }
        else
        {
            // TODO: Compound, arrays, collections etc
            throw new NotImplementedException($"Support for {typeof(TA).FullName} is not implemented.");
        }
    }

    public void WriteAttribute<TA>(
        [DisallowNull] string name,
        [DisallowNull] IEnumerable<TA> values,
        long[]? dimensions = null,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(values);
        // TODO: check no elements of values is null

        var exists = CheckExists(name, writeBehaviour);

        Func<H5Space> shapeCtor = dimensions == null
            ? (() => H5Space.Create(new Dimension(values.Count())))
            : (() => H5Space.Create(dimensions));

        // TODO: check values.Count = dimensions

        var type = typeof(TA);

        if (values is IEnumerable<string> stringValues)
        {
            WriteAttribute(name, stringValues, 0, writeBehaviour: writeBehaviour);
        }
        //else if (type.IsEnum)
        //{
        //    using var attribute =
        //        H5AAdapter.CreateOrOpen(this, name, H5EnumType<TA>.Create, shapeCtor, h => new H5EnumAttribute<TA>(h));

        //    attribute.Write(value);
        //}
        else if (values is IEnumerable<bool> boolValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5BooleanType.Create, shapeCtor, H5BooleanAttribute.Create);

            attribute.Write(boolValues);
        }
        else if (values is IEnumerable<decimal> decimalValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DecimalType.Create, shapeCtor, H5DecimalAttribute.Create);

            attribute.Write(decimalValues);
        }
        else if (values is IEnumerable<DateTimeOffset> dateTimeOffsetValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateTimeOffsetType.Create, shapeCtor, H5DateTimeOffsetAttribute.Create);

            attribute.Write(dateTimeOffsetValues);
        }
        else if (values is IEnumerable<DateTime> dateTimeValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateTimeType.Create, shapeCtor, H5DateTimeAttribute.Create);

            attribute.Write(dateTimeValues);
        }
        else if (values is IEnumerable<TimeSpan> timeSpanValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5TimeSpanType.Create, shapeCtor, H5TimeSpanAttribute.Create);

            attribute.Write(timeSpanValues);
        }
#if NET7_0_OR_GREATER
        else if (values is IEnumerable<TimeOnly> timeOnlyValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5TimeOnlyType.Create, shapeCtor, H5TimeOnlyAttribute.Create);

            attribute.Write(timeOnlyValues);
        }
        else if (values is IEnumerable<DateOnly> dateOnlyValues)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5DateOnlyType.Create, shapeCtor, H5DateOnlyAttribute.Create);

            attribute.Write(dateOnlyValues);
        }
#endif
        else if (type.GetElementType()?.IsPrimitive ?? true)
        {
            using var attribute =
                H5AAdapter.CreateOrOpen(this, name, H5PrimitiveType<TA>.Create, shapeCtor, H5PrimitiveAttribute<TA>.Create);

            attribute.Write(values);
        }
        else
        {
            // TODO: Compound, arrays, collections etc
            throw new NotImplementedException($"Support for {typeof(TA).FullName} is not implemented.");
        }
    }

    public void WriteUnmanagedAttribute<TA>(
        [DisallowNull] string name,
        [DisallowNull] IEnumerable<TA> values,
        long[]? dimensions = null,
        AttributeWriteBehaviour? writeBehaviour = null) where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(values);
        // TODO: check no elements of values is null

        var exists = CheckExists(name, writeBehaviour);

        Func<H5Space> shapeCtor = dimensions == null
            ? (() => H5Space.Create(new Dimension(values.Count())))
            : (() => H5Space.Create(dimensions));

        // TODO: check values.Count = dimensions

        var type = typeof(TA);

        using var attribute =
            H5AAdapter.CreateOrOpen(this, name, H5PrimitiveType<TA>.Create, shapeCtor, H5UnmanagedPrimitiveAttribute<TA>.Create);

        attribute.Write(values);
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
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(value);

        var exists = CheckExists(name, writeBehaviour);

        var shapeCtor = H5Space.CreateScalar; // TODO: arrays?

        using H5StringAttribute attribute = exists
            ? H5AAdapter.Open(this, name, H5StringAttribute.Create)
            : H5AAdapter.Create(this, name,
                () => H5StringType.Create(fixedStorageLength, characterSet, padding),
                shapeCtor, H5StringAttribute.Create);

        attribute.Write(value);
    }

    public void WriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] IEnumerable<string> values,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(values);
        // TODO: check no elements of values is null

        var exists = CheckExists(name, writeBehaviour);
        var shapeCtor = () => H5Space.Create(new Dimension(values.Count()));

        using H5StringAttribute attribute = exists
            ? H5AAdapter.Open(this, name, H5StringAttribute.Create)
            : H5AAdapter.Create(this, name,
                () => H5StringType.Create(fixedStorageLength, characterSet, padding),
                shapeCtor, H5StringAttribute.Create);

        attribute.Write(values);
    }

    public void WriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] IEnumerable<string> values,
        long[] dimensions,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(values);
        // TODO: check no elements of values is null

        var exists = CheckExists(name, writeBehaviour);
        var shapeCtor = () => H5Space.Create(dimensions);
        using var shape = shapeCtor();
        Debug.WriteLine($"{shape.GetSimpleExtentNPoints()}, {values.Count()}");
        if (shape.GetSimpleExtentNPoints() != values.Count())
        {
            // throw
            throw new H5Exception($"The dimensions specify a size of {shape.GetSimpleExtentNPoints()}, but there are {values.Count()} values.");
        }

        using H5StringAttribute attribute = exists
            ? H5AAdapter.Open(this, name, H5StringAttribute.Create)
            : H5AAdapter.Create(this, name,
                () => H5StringType.Create(fixedStorageLength, characterSet, padding),
                shapeCtor, H5StringAttribute.Create);

        attribute.Write(values);
    }

    public void WriteAttribute(
        [DisallowNull] string name,
        [DisallowNull] Array values,
        int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad,
        AttributeWriteBehaviour? writeBehaviour = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(values);
        // TODO: check no elements of values is null

        var dimensions = Enumerable.Range(0, values.Rank).Select(r => (long)values.GetUpperBound(r) + 1).ToArray();
        Debug.WriteLine(string.Join(",", dimensions));
        WriteAttribute(name, values.OfType<string>(), dimensions, fixedStorageLength, characterSet, padding, writeBehaviour);
    }

    #endregion
}
