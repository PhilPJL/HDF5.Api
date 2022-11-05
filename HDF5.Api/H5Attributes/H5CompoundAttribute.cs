using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5CompoundAttribute<T> : H5Attribute<T, H5CompoundAttribute<T>, H5CompoundType<T>>
{
    internal H5CompoundAttribute(long handle) : base(handle)
    {
    }

    public override H5CompoundType<T> GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5CompoundType<T>(h));
    }

    public override T Read()
    {
        throw new NotImplementedException();
    }

    public override H5CompoundAttribute<T> Write([DisallowNull] T value)
    {
        throw new NotImplementedException();
    }
}