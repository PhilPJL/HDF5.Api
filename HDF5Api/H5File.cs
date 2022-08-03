using System;
using System.IO;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5F (File) API.
/// </summary>
public class H5File : H5Location<H5FileHandle>
{
    private H5File(Handle handle) : base(new H5FileHandle(handle)) { }

    public long GetObjectCount(H5ObjectTypes types = H5ObjectTypes.All)
    {
        return GetObjectCount(this, types);
    }

    #region C level API wrappers

    /// <summary>
    ///     Create a new file. Truncates existing file.
    /// </summary>
    public static H5File Create(string path)
    {
        Handle h = H5F.create(path, H5F.ACC_TRUNC);

        h.ThrowIfNotValid("H5F.create");

        return new H5File(h);
    }

    /// <summary>
    ///     Open an existing file read-only
    /// </summary>
    public static H5File OpenReadOnly(string path)
    {
        return Open(path, true);
    }

    /// <summary>
    ///     Open an existing file read-write
    /// </summary>
    public static H5File OpenReadWrite(string path)
    {
        return Open(path, false);
    }

    public static H5File Open(string path, bool readOnly)
    {
        Handle h = H5F.open(path, readOnly ? H5F.ACC_RDONLY:  H5F.ACC_RDWR);

        h.ThrowIfNotValid("H5F.open");

        return new H5File(h);
    }

    public static H5File OpenOrCreate(string path, bool readOnly)
    {
        return File.Exists(path) ? Open(path, readOnly) : Create(path);
    }

    public static long GetObjectCount(H5FileHandle fileId, H5ObjectTypes types = H5ObjectTypes.All)
    {
        var ptr = H5F.get_obj_count(fileId, (uint)types);
        return (long)ptr;
    }

    #endregion
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
