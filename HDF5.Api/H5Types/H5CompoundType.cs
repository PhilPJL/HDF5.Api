using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5CompoundType<T> : H5Type<T>
{
    internal H5CompoundType(long handle) : base(handle)
    {
    }

    internal H5CompoundType<T> Insert([DisallowNull] string name, int offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, new ssize_t(offset), dataType);
        return this;
    }

    internal H5CompoundType<T> Insert([DisallowNull] string name, ssize_t offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, offset, dataType);
        return this;
    }

    internal H5CompoundType<T> Insert<TS, TP>([DisallowNull] string name) where TS : struct where TP : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        var offset = Marshal.OffsetOf<TS>(name);
        using var type = GetEquivalentNativeType<TP>();
        H5TAdapter.Insert(this, name, offset, type);
        return this;
    }
}
