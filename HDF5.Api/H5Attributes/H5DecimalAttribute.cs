﻿using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DecimalAttribute : H5Attribute<decimal, H5DecimalAttribute, H5DecimalType>
{
    internal H5DecimalAttribute(long handle) : base(handle)
    {
    }

    public override H5DecimalType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5DecimalType(h));
    }

    public override decimal Read(bool verifyType = false)
    {
        // TODO: implement
        throw new NotImplementedException();
    }

    public override H5DecimalAttribute Write([DisallowNull] decimal value)
    {
        // TODO: implement
        throw new NotImplementedException();
    }
}

