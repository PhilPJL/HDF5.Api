using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

#if NET7_0_OR_GREATER

public class H5DateOnlyAttribute : H5Attribute<DateOnly, H5DateOnlyAttribute, H5DateOnlyType>
{
    internal H5DateOnlyAttribute(long handle) : base(handle)
    {
    }

    public override H5DateOnlyType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5DateOnlyType(h));
    }

    public override DateOnly Read(bool verifyType = false)
    {
        using var type = GetAttributeType();
        using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();

        // TODO: sort out the type/expectedType/cls stuff

        int dayNumber = H5AAdapter.ReadImpl<int>(this, type, expectedType);

        return DateOnly.FromDayNumber(dayNumber);
    }

    public override void Write([DisallowNull] DateOnly value)
    {
        // TODO: optionally write value.ToString("O", CultureInfo.InvariantCulture)

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.DayNumber);
    }

    public static H5DateOnlyAttribute Create(long handle)
    {
        return new H5DateOnlyAttribute(handle);
    }
}

#endif
