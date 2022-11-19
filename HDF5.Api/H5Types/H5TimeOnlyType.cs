﻿using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

#if NET7_0_OR_GREATER

internal class H5TimeOnlyType : H5Type<TimeOnly>
{
    internal H5TimeOnlyType(long handle) : base(handle)
    {
    }

    internal static H5TimeOnlyType Create()
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5Type<long, H5TimeOnlyType>(static h => new H5TimeOnlyType(h));
    }
}

#endif
