using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

#if NET7_0_OR_GREATER

public class H5TimeOnlyAttribute : H5Attribute<TimeOnly, H5TimeOnlyAttribute, H5TimeOnlyType>
{
    internal H5TimeOnlyAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeOnlyType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5TimeOnlyType(h));
    }

    public override TimeOnly Read()
    {
        using var type = GetAttributeType();
        using var expectedType = H5TimeOnlyType.Create();

        long ticks = H5AAdapter.ReadImpl<long>(this, type, expectedType);

        return new TimeOnly(ticks);
    }

    public override void Write([DisallowNull] TimeOnly value)
    {
        // TODO: optionally write value.ToString("O", CultureInfo.InvariantCulture)

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.Ticks);
    }

    public static H5TimeOnlyAttribute Create(long handle)
    {
        return new H5TimeOnlyAttribute(handle);
    }
}

#endif
