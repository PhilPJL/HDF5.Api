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

    public override TimeOnly Read()
    {
        throw new NotImplementedException();
    }

    public override H5TimeOnlyAttribute Write([DisallowNull] TimeOnly value)
    {
        throw new NotImplementedException();
    }
}

#endif
