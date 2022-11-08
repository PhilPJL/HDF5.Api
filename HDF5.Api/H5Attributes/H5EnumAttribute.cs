using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5EnumAttribute<T> : H5Attribute<T, H5EnumAttribute<T>, H5EnumType<T>>
    //where T : Enum // unmanaged
{
    internal H5EnumAttribute(long handle) : base(handle)
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();
    }

    public override H5EnumType<T> GetAttributeType()
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();

        return H5AAdapter.GetType(this, h => new H5EnumType<T>(h));
    }

    public override T Read(bool verifyType = false)
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();

        using var nativeType = H5TAdapter.ConvertDotNetEnumUnderlyingTypeToH5NativeType<T>();
        using var type = GetAttributeType();

        if (verifyType)
        {
            using var enumType = H5EnumType<T>.Create();

            if (!enumType.IsEqualTo(type))
            {
                throw new H5Exception($"{Name} does not have an equivalent HDF5 enumeration of {typeof(T)}.");
            }
        }

        return H5AAdapter.ReadImpl<T>(this, type, nativeType);
    }

    public override void Write(T value)
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();
        Guard.IsNotNull(value);

        WriteEnum();

        H5Attribute WriteEnum()
        {
            return ((Enum)(object)value).GetTypeCode() switch
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
            using var enumType = GetAttributeType();
            H5AAdapter.Write(this, enumType, value);

            return this;
        }
    }

    public static H5EnumAttribute<T> Create(long handle)
    {
        return new H5EnumAttribute<T>(handle);
    }
}
