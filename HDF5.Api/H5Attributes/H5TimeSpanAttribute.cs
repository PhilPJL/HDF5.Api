using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5TimeSpanAttribute : H5Attribute<TimeSpan, H5TimeSpanAttribute, H5TimeSpanType>
{
    internal H5TimeSpanAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeSpanType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5TimeSpanType(h));
    }

    public override TimeSpan Read(bool verifyType = false)
    {
        // TODO: optionally write value.ToString("G", CultureInfo.InvariantCulture)
        using var type = GetAttributeType();

        // TODO: sort out the type/expectedType/cls stuff
        long timeSpan = H5AAdapter.ReadImpl<long>(this, type, type);
        return TimeSpan.FromTicks(timeSpan);
    }

    public override H5TimeSpanAttribute Write([DisallowNull] TimeSpan value)
    {
        // TODO: optionally write value.ToString("G", CultureInfo.InvariantCulture)
        /*        if (...)
                {
                    // Write fixed length string
                    Write(attribute, value.ToString("G", CultureInfo.InvariantCulture))
                }*/

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.Ticks);

        return this;
    }
}

