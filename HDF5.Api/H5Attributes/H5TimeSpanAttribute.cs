using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

internal class H5TimeSpanAttribute : H5Attribute<TimeSpan, H5TimeSpanAttribute, H5TimeSpanType>
{
    internal H5TimeSpanAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeSpanType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5TimeSpanType(h));
    }

    public override TimeSpan Read()
    {
        // TODO: optionally write value.ToString("G", CultureInfo.InvariantCulture)
        using var type = GetAttributeType();

        // TODO: sort out the type/expectedType/cls stuff
        long timeSpan = H5AAdapter.ReadImpl<long>(this, type, type);
        return TimeSpan.FromTicks(timeSpan);
    }

    public override void Write([DisallowNull] TimeSpan value)
    {
        // TODO: optionally write value.ToString("G", CultureInfo.InvariantCulture)
        /*        if (...)
                {
                    // Write fixed length string
                    Write(attribute, value.ToString("G", CultureInfo.InvariantCulture))
                }*/

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.Ticks);
    }

    public static H5TimeSpanAttribute Create(long handle)
    {
        return new H5TimeSpanAttribute(handle);
    }
}

