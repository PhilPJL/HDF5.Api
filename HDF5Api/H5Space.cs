using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5S (Space) API.
/// </summary>
public class H5Space : H5Object<H5Space>
{
    internal H5Space(long handle) : base(handle, H5SAdapter.Close)
    {
    }

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

    public static H5Space CreateSimple(params long[] dimensions)
    {
        return Create(dimensions.Select(d => new Dimension(d)).ToArray());
    }
}
