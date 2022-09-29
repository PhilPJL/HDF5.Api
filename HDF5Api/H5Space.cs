using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5S (Space) API.
/// </summary>
public class H5Space : H5Object<H5Space>
{
    internal H5Space(long handle) : base(handle, H5SpaceNativeMethods.Close)
    {
    }

    #region Public Api

    public void SelectHyperSlab(int offset, int count)
    {
        H5SpaceNativeMethods.SelectHyperSlab(this, offset, count);
    }

    public long GetSimpleExtentNPoints()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNPoints(this);
    }

    public int GetSimpleExtentNDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNDims(this);
    }

    public IReadOnlyList<Dimension> GetSimpleExtentDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentDims(this);
    }

    #endregion

    public static H5Space Create(params Dimension[] dimensions)
    {
        return H5SpaceNativeMethods.CreateSimple(dimensions);
    }
}
