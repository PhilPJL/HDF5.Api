using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

#if NET7_0_OR_GREATER

public class H5TimeOnlyAttribute : H5Attribute<TimeOnly, H5TimeOnlyAttribute, H5TimeOnlyType>
{
    internal H5TimeOnlyAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeOnlyType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5TimeOnlyType(h));
    }

    public override TimeOnly Read(bool verifyType = false)
    {
        using var type = GetH5Type();
        using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();

        // TODO: sort out the type/expectedType/cls stuff

        long ticks = H5AAdapter.ReadImpl<long>(this, type, expectedType);

        return new TimeOnly(ticks);
    }

    public override H5TimeOnlyAttribute Write([DisallowNull] TimeOnly value)
    {
        // TODO: optionally write value.ToString("O", CultureInfo.InvariantCulture)

        //using var type = GetH5Type();
        //using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();

        H5AAdapter.WritePrimitive(this, value.Ticks);

        return this;
    }
}

#endif
