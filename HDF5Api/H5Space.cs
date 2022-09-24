﻿namespace HDF5Api;

/// <summary>
///     Wrapper for H5S (Space) API.
/// </summary>
public class H5Space : H5Object<H5Space>
{
    internal H5Space(long handle) : base(handle, H5SpaceNativeMethods.Close)
    {
    }

    #region Public Api

    public void SelectHyperslab(int offset, int count)
    {
        H5SpaceNativeMethods.SelectHyperslab(this, offset, count);
    }

    public long GetSimpleExtentNPoints()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNPoints(this);
    }

    public int GetSimpleExtentNDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNDims(this);
    }

    public (int rank, ulong[] dims, ulong[] maxDims) GetSimpleExtentDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentDims(this);
    }

    #endregion
}
