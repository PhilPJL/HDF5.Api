using HDF5.Api.H5Types;

namespace HDF5.Api.H5Attributes;

public abstract class H5Attribute<T, TA, TT> : H5Attribute
    where TA : H5Attribute<T, TA, TT>
    where TT : H5Type<T>
{
    internal H5Attribute(long handle) : base(handle)
    {
    }

    public abstract TT GetAttributeType();

    public abstract T Read(bool verifyType = false);

    // TODO: is there any point in returning TA?
    public abstract TA Write([DisallowNull] T value);

    [DisallowNull]
    public T Value
    {
        get => Read();
        set => Write(value);
    }
}
