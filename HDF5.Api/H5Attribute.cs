using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.Utils;
using System.Diagnostics;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public abstract class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public string Name => H5AAdapter.GetName(this);

    internal H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    internal int StorageSize => H5AAdapter.GetStorageSize(this);

    /*    #region Read

        public T Read<T>()
        {
            if(typeof(T) == typeof(string))
            {
                return (T)ReadString();
            }

            return default(T) switch
            {
                // primitive
                bool => ReadPrimitive(),
                char => ReadPrimitive(),
                sbyte => ReadPrimitive(),
                byte => ReadPrimitive(),
                short => ReadPrimitive(),
                ushort => ReadPrimitive(),
                int => ReadPrimitive(),
                uint => ReadPrimitive(),
                long => ReadPrimitive(),
                ulong => ReadPrimitive(),
                float => ReadPrimitive(),
                double => ReadPrimitive(),

                // decimal
                decimal => (T)ReadDecimal(),

                // enum
                Enum => ReadEnum(),

                // date & time
                DateTime => (T)ReadDateTime(),
                DateTimeOffset => (T)ReadDateTimeOffset(),
                TimeSpan => (T)ReadTimeSpan(),

                // everything else
                _ => ReadCompound()
            };

            T ReadPrimitive()
            {
                T value = H5AAdapter.Read<T>(this);
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                return value;
            }

            object ReadString()
            {
                return H5AAdapter.ReadString(this);
            }

            T ReadEnum(bool verifyType = false)
            {
                return H5AAdapter.ReadEnum<T>(this, verifyType);
            }

            object ReadDateTime()
            {
                return DateTime.FromBinary(H5AAdapter.Read<long>(this));
            }

            object ReadDateTimeOffset()
            {
                return H5AAdapter.ReadDateTimeOffset(this);
            }

            object ReadTimeSpan()
            {
                return new TimeSpan(H5AAdapter.Read<long>(this));
            }

            object ReadDecimal()
            {
                throw new NotImplementedException();
            }

            T ReadCompound()
            {
                if (typeof(T).IsUnmanaged())
                {
                    return ReadCompoundUnmanaged();
                }

                throw new NotImplementedException();

                T ReadCompoundUnmanaged()
                {
                    // TODO: optimised struct based
                    throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region Write

        public H5Attribute Write<T>([DisallowNull] T value)
        {
            Guard.IsNotNull(value);

            return value switch
            {
                // primitive
                bool number => WritePrimitive(number),
                char number => WritePrimitive(number),
                sbyte number => WritePrimitive(number),
                byte number => WritePrimitive(number),
                short number => WritePrimitive(number),
                ushort number => WritePrimitive(number),
                int number => WritePrimitive(number),
                uint number => WritePrimitive(number),
                long number => WritePrimitive(number),
                ulong number => WritePrimitive(number),
                float number => WritePrimitive(number),
                double number => WritePrimitive(number),

                // enum
                Enum @enum => WriteEnum(@enum),

                // decimal
                decimal number => WriteDecimal(number),

                // string
                string s => WriteString(s),

                // date & time
                TimeSpan timeSpan => WriteTimeSpan(timeSpan),
                DateTime dateTime => WriteDateTime(dateTime),
                DateTimeOffset dateTimeOffset => WriteDateTimeOffset(dateTimeOffset),

                // everything else
                _ => WriteCompound(value)
            };

            H5Attribute WriteDecimal(decimal value)
            {
                // TODO: decimal is not primitive and is 16 bytes
                throw new NotImplementedException();
            }

            H5Attribute WriteTimeSpan(TimeSpan value)
            {
                // TODO: optionally write ticks or value.ToString("G", CultureInfo.InvariantCulture)
                return WritePrimitive(value.Ticks);
            }

            H5Attribute WriteDateTime(DateTime value)
            {
                // TODO: optionally write binary (ticks + kind) or value.ToString("O")
                return WritePrimitive(value.ToBinary());
            }

            H5Attribute WriteDateTimeOffset(DateTimeOffset value)
            {
                H5AAdapter.Write(this, value);
                return this;
            }

            H5Attribute WritePrimitive<TP>(TP value) where TP : unmanaged
            {
                H5AAdapter.Write(this, value);
                return this;
            }

            H5Attribute WriteEnum(Enum value)
            {
                return value.GetTypeCode() switch
                {
                    TypeCode.Byte => WritePrimitive(Convert.ToByte(value)),
                    TypeCode.SByte => WritePrimitive(Convert.ToSByte(value)),
                    TypeCode.Int16 => WritePrimitive(Convert.ToInt16(value)),
                    TypeCode.UInt16 => WritePrimitive(Convert.ToUInt16(value)),
                    TypeCode.Int32 => WritePrimitive(Convert.ToInt32(value)),
                    TypeCode.UInt32 => WritePrimitive(Convert.ToUInt32(value)),
                    TypeCode.Int64 => WritePrimitive(Convert.ToInt64(value)),
                    TypeCode.UInt64 => WritePrimitive(Convert.ToUInt64(value)),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private H5Attribute WriteCompound<T>(T value)
        {
            if (typeof(T).IsUnmanaged())
            {
                return WriteCompoundUnmanaged(value);
            }

            // TODO: class based
            throw new NotImplementedException();

            H5Attribute WriteCompoundUnmanaged(T value)
            {
                // TODO: optimised struct based
                throw new NotImplementedException();
            }
        }

        #endregion
    */
}

public abstract class H5Attribute<T> : H5Attribute
{
    internal H5Attribute(long handle) : base(handle)
    {
    }

    [Conditional("DEBUG")]
    internal void AssertIsUnmanaged()
    {
        if (!typeof(T).IsUnmanaged())
        {
            // TODO: improve message
            throw new InvalidOperationException($"{typeof(T).Name} is managed.");
        }
    }

    public abstract H5Attribute<T>
        //#else
        //    public abstract H5Attribute
        //#endif
        Write([DisallowNull] T value);

    //#if NET7_0_OR_GREATER
    public abstract H5Type<T>
    //#else
    //    public virtual H5Type
    //#endif
    GetH5Type();
    //{
    //    return H5AAdapter.GetType(this, h => new H5Type<T>(h));
    //}

    public abstract T Read();

    [DisallowNull]
    public T Value
    {
        get => Read();
        set => Write(value);
    }
}

public class H5BooleanAttribute : H5Attribute<bool>
{
    internal H5BooleanAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5BooleanType GetH5Type()
#else
    public override H5Type<bool> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5BooleanType(h));
    }

    public override bool Read()
    {
        return H5AAdapter.ReadBool(this);
    }
#if NET7_0_OR_GREATER
    public override H5BooleanAttribute Write([DisallowNull] bool value)
#else
    public override H5Attribute<bool> Write([DisallowNull] bool value)
#endif
    {
        Guard.IsNotNull(value);

        H5AAdapter.WriteBool(this, value);

        return this;
    }
}

public class H5PrimitiveAttribute<T> : H5Attribute<T> //where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5PrimitiveType<T> GetH5Type()
#else
    public override H5Type<T> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        return H5AAdapter.Read<T>(this);
    }

    public override H5Attribute<T> Write([DisallowNull] T value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}

public class H5EnumAttribute<T> : H5Attribute<T>
    where T : Enum // unmanaged
{
    internal H5EnumAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5EnumType<T>
#else
    public override H5Type<T>
#endif
    GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5EnumType<T>(h));
    }

    public override T Read()
    {
        return H5AAdapter.ReadEnum<T>(this);
    }

    public override H5Attribute<T> Write(T value)
    {
        Guard.IsNotNull(value);

        WriteEnum();

        return this;

        H5Attribute WriteEnum()
        {
            return value.GetTypeCode() switch
            {
                TypeCode.Byte => WritePrimitive(Convert.ToByte(value)),
                TypeCode.SByte => WritePrimitive(Convert.ToSByte(value)),
                TypeCode.Int16 => WritePrimitive(Convert.ToInt16(value)),
                TypeCode.UInt16 => WritePrimitive(Convert.ToUInt16(value)),
                TypeCode.Int32 => WritePrimitive(Convert.ToInt32(value)),
                TypeCode.UInt32 => WritePrimitive(Convert.ToUInt32(value)),
                TypeCode.Int64 => WritePrimitive(Convert.ToInt64(value)),
                TypeCode.UInt64 => WritePrimitive(Convert.ToUInt64(value)),
                _ => throw new NotImplementedException(),
            };
        }

        H5Attribute WritePrimitive<TP>(TP value) where TP : unmanaged
        {
            H5AAdapter.WritePrimitive(this, value);
            return this;
        }
    }
}

public class H5StringAttribute : H5Attribute<string>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
        /*        using var type = GetH5Type();

                var typeClass = type.GetClass();

                if(typeClass != DataTypeClass.String)
                {
                    throw new H5Exception($"The attribute should be of class {DataTypeClass.String} but is of class {typeClass}.");
                }*/
    }

#if NET7_0_OR_GREATER
    public override H5StringType GetH5Type()
#else
    public override H5Type<string> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5StringType(h));
    }

    public override string Read()
    {
        return H5AAdapter.ReadString(this);
    }

#if NET7_0_OR_GREATER
    public override H5StringAttribute Write([DisallowNull] string value)
#else
    public override H5Attribute<string> Write([DisallowNull] string value)
#endif
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}

public class H5DecimalAttribute : H5Attribute<decimal>
{
    internal H5DecimalAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5DecimalType GetH5Type()
#else
    public override H5Type<decimal> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5DecimalType(h));
    }

    public override decimal Read()
    {
        throw new NotImplementedException();
    }

    public override H5Attribute<decimal> Write([DisallowNull] decimal value)
    {
        throw new NotImplementedException();
    }
}

public class H5TimeSpanAttribute : H5Attribute<TimeSpan>
{
    internal H5TimeSpanAttribute(long handle) : base(handle)
    {
    }
#if NET7_0_OR_GREATER
    public override H5TimeSpanType GetH5Type()
#else
    public override H5Type<TimeSpan> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5TimeSpanType(h));
    }

    public override TimeSpan Read()
    {
        throw new NotImplementedException();
    }

    public override H5Attribute<TimeSpan> Write([DisallowNull] TimeSpan value)
    {
        throw new NotImplementedException();
    }
}

#if NET7_0_OR_GREATER
public class H5TimeOnlyAttribute : H5Attribute<TimeOnly>
{
    internal H5TimeOnlyAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeOnlyType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5TimeOnlyType(h));
    }

    public override TimeOnly Read()
    {
        throw new NotImplementedException();
    }

    public override H5Attribute<TimeOnly> Write([DisallowNull] TimeOnly value)
    {
        throw new NotImplementedException();
    }
}
#endif

public class H5DateTimeAttribute : H5Attribute<DateTime>
{
    internal H5DateTimeAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5DateTimeType GetH5Type()
#else
    public override H5Type<DateTime> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5DateTimeType(h));
    }

    public override DateTime Read()
    {
        throw new NotImplementedException();
    }

    public override H5Attribute<DateTime> Write([DisallowNull] DateTime value)
    {
        throw new NotImplementedException();
    }
}

public class H5DateTimeOffsetAttribute : H5Attribute<DateTimeOffset>
{
    internal H5DateTimeOffsetAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5DateTimeOffsetType GetH5Type()
#else
    public override H5Type<DateTimeOffset> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5DateTimeOffsetType(h));
    }

    public override DateTimeOffset Read()
    {
        throw new NotImplementedException();
    }

    public override H5Attribute<DateTimeOffset> Write([DisallowNull] DateTimeOffset value)
    {
        throw new NotImplementedException();
    }
}

public class H5CompoundAttribute<T> : H5Attribute<T>
{
    internal H5CompoundAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5CompoundType<T> GetH5Type()
#else
    public override H5Type<T> GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5CompoundType<T>(h));
    }

    public override T Read()
    {
        throw new NotImplementedException();
    }

#if NET7_0_OR_GREATER
    public override H5CompoundAttribute<T> Write([DisallowNull] T value)
#else
    public override H5Attribute<T> Write([DisallowNull] T value)
#endif
    {
        throw new NotImplementedException();
    }
}