using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DecimalAttribute : H5Attribute<decimal, H5DecimalAttribute, H5DecimalType>
{
    internal H5DecimalAttribute(long handle) : base(handle)
    {
    }

    public override H5DecimalType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5DecimalType(h));
    }

    public override decimal Read()
    {
        using var type = GetAttributeType();
        using var expectedType = H5DecimalType.Create();

        return H5AAdapter.ReadImpl<decimal>(this, type, expectedType);
    }

    public override void Write([DisallowNull] decimal value)
    {
        using var type = GetAttributeType();
        H5AAdapter.Write(this, type, value);
    }

    public static H5DecimalAttribute Create(long handle)
    {
        return new H5DecimalAttribute(handle);
    }
}

