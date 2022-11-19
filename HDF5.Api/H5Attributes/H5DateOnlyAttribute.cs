using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

#if NET7_0_OR_GREATER

internal class H5DateOnlyAttribute : H5Attribute<DateOnly, H5DateOnlyAttribute, H5DateOnlyType>
{
    internal H5DateOnlyAttribute(long handle) : base(handle)
    {
    }

    public override H5DateOnlyType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5DateOnlyType(h));
    }

    public override DateOnly Read()
    {
        using var type = GetAttributeType();
        using var expectedType = H5DateOnlyType.Create();

        int dayNumber = H5AAdapter.ReadImpl<int>(this, type, expectedType);
        return DateOnly.FromDayNumber(dayNumber);
    }

    public override void Write([DisallowNull] DateOnly value)
    {
        // TODO: optionally write value.ToString("O", CultureInfo.InvariantCulture)

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<int>();
        H5AAdapter.Write(this, type, value.DayNumber);
    }

    public static H5DateOnlyAttribute Create(long handle)
    {
        return new H5DateOnlyAttribute(handle);
    }
}

#endif
