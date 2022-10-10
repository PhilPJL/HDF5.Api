using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5S (Space) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_s.html"/>
/// </summary>
public class H5Space : H5Object<H5Space>
{
    internal H5Space(long handle) : base(handle, HandleType.Space, H5SAdapter.Close) { }

    #region Public Api

    public void SelectHyperslab(long offset, long count)
    {
        H5SAdapter.SelectHyperslab(this, offset, count);
    }

    public long GetSimpleExtentNPoints()
    {
        return H5SAdapter.GetSimpleExtentNPoints(this);
    }

    public int GetSimpleExtentNDims()
    {
        return H5SAdapter.GetSimpleExtentNDims(this);
    }

    public IReadOnlyList<Dimension> GetSimpleExtentDims()
    {
        return H5SAdapter.GetSimpleExtentDims(this);
    }

    #endregion

    public static H5Space Create([DisallowNull] params Dimension[] dimensions)
    {
        Guard.IsNotNull(dimensions);

        return H5SAdapter.CreateSimple(dimensions);
    }

    public static H5Space Create(params long[] dimensions)
    {
        return Create(dimensions.Select(d => new Dimension(d)).ToArray());
    }

    public static H5Space Create(params (long initialSize, long upperLimit)[] dimensions)
    {
        return Create(dimensions.Select(d => new Dimension(d.initialSize, d.upperLimit)).ToArray());
    }
}
