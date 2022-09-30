using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using HDF5Api.NativeMethods;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5F (File) API.
/// </summary>
public class H5File : H5Location<H5File>
{
    public H5File(long handle) : base(handle, H5FileNativeMethods.Close)
    {
    }

    public long GetObjectCount(H5ObjectTypes types = H5ObjectTypes.All)
    {
        return H5FileNativeMethods.GetObjectCount(this, types);
    }

    /// <summary>
    ///     Open an existing file.  By default opens read-write.
    /// </summary>
    public static H5File Open([DisallowNull] string path, bool readOnly = false)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FileNativeMethods.Open(path, readOnly);
    }

    /// <summary>
    ///     Open an existing file (by default read-write) or create new.
    /// </summary>
    public static H5File OpenOrCreate([DisallowNull] string path, bool readOnly = false)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return File.Exists(path)
            ? H5FileNativeMethods.Open(path, readOnly)
            : H5FileNativeMethods.Create(path);
    }

    public static H5File Create([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FileNativeMethods.Create(path);
    }
}

[Flags]
public enum H5ObjectTypes : uint
{
    All = H5F.OBJ_ALL,
    Attribute = H5F.OBJ_ATTR,
    DataSet = H5F.OBJ_DATASET,
    DataType = H5F.OBJ_DATATYPE,
    Group = H5F.OBJ_GROUP,
    Local = H5F.OBJ_LOCAL
}
