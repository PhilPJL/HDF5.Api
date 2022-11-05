using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public abstract class H5Type<T> : H5Type
{
    internal H5Type(long handle) : base(handle)
    {
    }
}
