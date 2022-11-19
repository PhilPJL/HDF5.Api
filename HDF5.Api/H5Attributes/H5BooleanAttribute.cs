using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

internal class H5BooleanAttribute : H5Attribute<bool, H5BooleanAttribute, H5BooleanType>
{
    internal H5BooleanAttribute(long handle) : base(handle)
    {
    }

    public override H5BooleanType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5BooleanType(h));
    }

    public override bool Read()
    {
        using var type = GetAttributeType();
        using var expectedType = H5BooleanType.Create(); 

        return H5AAdapter.ReadImpl<H5BooleanType.Boolean>(this, type, expectedType) == H5BooleanType.Boolean.TRUE;
    }

    public override void Write([DisallowNull] bool value)
    {
        using var type = H5BooleanType.Create(); 
        H5AAdapter.Write(this, type, value); 
    }

    public static H5BooleanAttribute Create(long handle)
    {
        return new H5BooleanAttribute(handle);
    }
}
