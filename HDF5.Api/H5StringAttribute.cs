using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5StringAttribute : H5Attribute<string>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
/*        using var type = GetH5Type();

        var typeClass = type.GetClass();

        if(typeClass != DataTypeClass.String)
        {
            throw new H5Exception($"The attribute should be of class {DataTypeClass.String} but is of class {typeClass}.");
        }*/
    }

#if NET7_0_OR_GREATER
    public override H5StringType GetH5Type()
#else
    public override H5Type GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5StringType(h));
    }

    public override string Read()
    {
        return H5AAdapter.ReadString(this);
    }

#if NET7_0_OR_GREATER
    public override H5StringAttribute Write([DisallowNull] string value)
#else
    public override H5Attribute<string> Write([DisallowNull] string value)
#endif
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}
