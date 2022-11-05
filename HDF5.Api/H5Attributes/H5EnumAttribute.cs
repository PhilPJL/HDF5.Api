using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5EnumAttribute<T> : H5Attribute<T, H5EnumAttribute<T>, H5EnumType<T>>
    where T : Enum // unmanaged
{
    internal H5EnumAttribute(long handle) : base(handle)
    {
    }

    public override H5EnumType<T> GetH5Type()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        return H5AAdapter.GetType(this, h => new H5EnumType<T>(h));
    }

    public override T Read(bool verifyType = false)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
        
        return H5AAdapter.ReadEnum<T>(this, verifyType);
    }

    public override H5EnumAttribute<T> Write(T value)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
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
            H5AAdapter.WriteEnum<T, TP>(this, value);
            return this;
        }
    }
}
