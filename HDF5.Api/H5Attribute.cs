using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

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

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public H5Type GetH5Type()
    {
        return H5AAdapter.GetType(this);
    }

    public string Name => H5AAdapter.GetName(this);

    public int StorageSize => H5AAdapter.GetStorageSize(this);

    #region Read

    public string ReadString()
    {
        return H5AAdapter.ReadString(this);
    }

    public T Read<T>() where T : unmanaged
    {
        return H5AAdapter.Read<T>(this);
    }

    public T ReadEnum<T>(bool verifyType = false) where T : unmanaged, Enum
    {
        return H5AAdapter.ReadEnum<T>(this, verifyType);
    }

    public DateTime ReadDateTime()
    {
        return DateTime.FromBinary(H5AAdapter.Read<long>(this));
    }

    public TimeSpan ReadTimeSpan()
    {
        return new TimeSpan(H5AAdapter.Read<long>(this));
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

            // decimal
            decimal number => WriteDecimal(number),

            // string
            string s => WriteString(s),

            // date & time
            TimeSpan timeSpan => WriteTimeSpan(timeSpan),
            DateTime dateTime => WriteDateTime(dateTime),
            DateTimeOffset dateTimeOffset => WriteDateTimeOffset(dateTimeOffset),
            Enum @enum => WriteEnum(@enum),

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
            // TODO: optionally write ticks + offset or value.ToString("O")
            throw new NotImplementedException();
        }

        H5Attribute WritePrimitive<TP>(TP value) where TP : unmanaged
        {
            H5AAdapter.Write(this, value);
            return this;
        }

        H5Attribute WriteString([DisallowNull] string value)
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
}

static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, bool> _memoized = new();

    public static bool IsUnmanaged(this Type type)
    {
        // check if we already know the answer
        if (!_memoized.TryGetValue(type, out var answer))
        {
            if (!type.IsValueType)
            {
                // not a struct -> false
                answer = false;
            }
            else if (type.IsPrimitive || type.IsPointer || type.IsEnum)
            {
                // primitive, pointer or enum -> true
                answer = true;
            }
            else
            {
                // otherwise check recursively
                answer = type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .All(f => IsUnmanaged(f.FieldType));
            }

            _memoized[type] = answer;
        }

        return answer;
    }
}