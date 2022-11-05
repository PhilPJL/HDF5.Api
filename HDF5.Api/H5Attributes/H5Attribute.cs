using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.Utils;
using System.Diagnostics;

namespace HDF5.Api.H5Attributes;

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

public abstract class H5Attribute<T, TA, TT> : H5Attribute
    where TA : H5Attribute<T, TA, TT>
    where TT : H5Type<T>
{
    internal H5Attribute(long handle) : base(handle)
    {
    }

    public abstract TA Write([DisallowNull] T value);

    public abstract TT GetH5Type();

    public abstract T Read();

    [DisallowNull]
    public T Value
    {
        get => Read();
        set => Write(value);
    }
}
