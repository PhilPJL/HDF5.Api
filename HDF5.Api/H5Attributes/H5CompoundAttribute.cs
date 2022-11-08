using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5CompoundAttribute<T> : H5Attribute<T, H5CompoundAttribute<T>, H5CompoundType<T>>
{
    internal H5CompoundAttribute(long handle) : base(handle)
    {
    }

    public override H5CompoundType<T> GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5CompoundType<T>(h));
    }

    public override T Read(bool verifyType = false)
    {
        throw new NotImplementedException();
    }

    public override void Write([DisallowNull] T value)
    {
        throw new NotImplementedException();
    }

    public static H5CompoundAttribute<T> Create(long handle)
    {
        return new H5CompoundAttribute<T>(handle);
    }
}