using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.H5Types;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5D (DataSet) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
public class H5DataSet : H5ObjectWithAttributes<H5DataSet>
{
    internal H5DataSet(long handle) : base(handle, HandleType.DataSet, H5DAdapter.Close)
    {
    }

    internal static H5DataSetCreationPropertyList CreateCreationPropertyList()
    {
        return H5DAdapter.CreateCreationPropertyList();
    }

    internal static H5DataSetAccessPropertyList CreateAccessPropertyList()
    {
        return H5DAdapter.CreateAccessPropertyList();
    }

    internal H5DataSetCreationPropertyList GetCreationPropertyList()
    {
        return H5DAdapter.GetCreationPropertyList(this);
    }

    internal H5DataSetAccessPropertyList GetAccessPropertyList()
    {
        return H5DAdapter.GetAccessPropertyList(this);
    }

    internal H5Type GetH5Type()
    {
        return H5DAdapter.GetType(this);
    }

    public string Name => H5IAdapter.GetName(this);

    internal H5Space GetSpace()
    {
        return H5DAdapter.GetSpace(this);
    }

    public Span<T> Read<T>() where T : unmanaged
    {
        return H5DAdapter.Read<T>(this);
    }

    public H5DataSet SetExtent([DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims);

        H5DAdapter.SetExtent(this, dims);
        return this;
    }

    // TODO: get rid of IntPtr
    internal H5DataSet Write([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, [DisallowNull] IntPtr buffer)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(memorySpace);
        Guard.IsNotNull(fileSpace);
        Guard.IsNotNull(buffer);

        H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer, null);
        return this;
    }
}