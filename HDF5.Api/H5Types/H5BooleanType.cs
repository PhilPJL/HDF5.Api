﻿using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5BooleanType : H5Type<bool>
{
    internal H5BooleanType(long handle) : base(handle)
    {
    }

    internal static H5BooleanType Create()
    {
        return H5TAdapter.CreateEnumType<Boolean, H5BooleanType>(h => new H5BooleanType(h));
    }

    // NOTE: this is compatible with h5py - there are other ways to encode boolean
    public enum Boolean : byte
    {
        FALSE = 0,
        TRUE = 1
    }
}
