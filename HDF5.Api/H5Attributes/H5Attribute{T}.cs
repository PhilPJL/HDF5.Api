using HDF5.Api.H5Types;
using System.Collections.Generic;

namespace HDF5.Api.H5Attributes;

internal abstract class H5Attribute<T, TA, TT> : H5Attribute
    where TA : H5Attribute<T, TA, TT>
    where TT : H5Type<T>
{
    internal H5Attribute(long handle) : base(handle)
    {
    }

    public abstract TT GetAttributeType();

    public abstract T Read();

    public virtual IEnumerable<T> ReadCollection()
    {
        return Array.Empty<T>();
    }

    public abstract void Write([DisallowNull] T value);

    public virtual void Write([DisallowNull] IEnumerable<T> value)
    {
        throw new NotImplementedException($"{typeof(IEnumerable<T>)}");
    }

    [DisallowNull]
    public T Value
    {
        get => Read();
        set => Write(value);
    }
}
