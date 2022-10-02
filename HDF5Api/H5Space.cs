using System.Collections.Generic;
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

    public void SelectHyperSlab(int offset, int count)
    {
        H5SAdapter.SelectHyperSlab(this, offset, count);
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

    public static H5Space Create(params Dimension[] dimensions)
    {
        return H5SAdapter.CreateSimple(dimensions);
    }
}
