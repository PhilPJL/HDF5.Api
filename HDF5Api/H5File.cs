using System;
using System.IO;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5F (File) API.
/// </summary>
public struct H5File : IDisposable
{
    #region Constructor and operators

    private long Handle { get; set; } = H5Handle.DefaultHandleValue;
    private readonly bool _isNative = false;

    internal H5File(long handle, bool isNative = false)
    {
        handle.ThrowIfDefaultOrInvalidHandleValue();

        Handle = handle;
        _isNative = isNative;

        if (!_isNative)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public void Dispose()
    {
        if (_isNative || Handle == H5Handle.DefaultHandleValue)
        {
            // native or default(0) handle shouldn't be disposed
            return;
        }

        if (Handle == H5Handle.InvalidHandleValue)
        {
            // already disposed
            // TODO: throw already disposed
        }

        // close and mark as disposed
        H5FileNativeMethods.Close(this);
        H5Handle.UntrackHandle(Handle);
        Handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5File h5object)
    {
        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }

    #endregion

    public long GetObjectCount(H5ObjectTypes types = H5ObjectTypes.All)
    {
        return H5FileNativeMethods.GetObjectCount(this, types);
    }

    /// <summary>
    ///     Open an existing file read-only
    /// </summary>
    public static H5File OpenReadOnly(string path)
    {
        return H5FileNativeMethods.Open(path, true);
    }

    /// <summary>
    ///     Open an existing file read-write
    /// </summary>
    public static H5File OpenReadWrite(string path)
    {
        return H5FileNativeMethods.Open(path, false);
    }

    public static H5File OpenOrCreate(string path, bool readOnly)
    {
        return File.Exists(path) 
            ? H5FileNativeMethods.Open(path, readOnly) 
            : H5FileNativeMethods.Create(path);
    }

    public static H5File Create(string path)
    {
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
