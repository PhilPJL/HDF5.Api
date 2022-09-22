using HDF5Api.Disposables;
using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5A (Attribute) API.
/// </summary>
public struct H5Attribute : IDisposable
{
    #region Constructor and operators

    private long Handle { get; set; } = H5Handle.DefaultHandleValue;
    private readonly bool _isNative = false;

    internal H5Attribute(long handle, bool isNative = false)
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
        if(_isNative || Handle == H5Handle.DefaultHandleValue)
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
        H5AttributeNativeMethods.Close(this);
        H5Handle.UntrackHandle(Handle);
        Handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5Attribute h5object)
    {
        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }

    #endregion

    #region Public Api

    public H5Space GetSpace()
    {
        return H5AttributeNativeMethods.GetSpace(this);
    }

    public string ReadString()
    {
        using var type = H5TypeNativeMethods.GetType(this);
        using var space = GetSpace();

        long count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();
        if (cls != H5T.class_t.STRING)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting STRING.");
        }

        long size = H5AttributeNativeMethods.GetStorageSize(this);

        // TODO: probably simpler way to do this.
        using var buffer = new GlobalMemory((int)(size + 1));
        int err = H5A.read(this, type, buffer.IntPtr);
        err.ThrowIfError("H5A.read");

        // TODO: marshal Ansi/UTF8/.. etc as appropriate
        return Marshal.PtrToStringAnsi(buffer.IntPtr, (int)size);
    }

    public DateTime ReadDateTime()
    {
        return DateTime.FromOADate(Read<double>());
    }

    public T Read<T>() where T : unmanaged
    {
        using var type = H5TypeNativeMethods.GetType(this);
        using var space = GetSpace();

        long count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();

        using var nativeType = H5Type.GetNativeType<T>();
        var expectedCls = H5TypeNativeMethods.GetClass(nativeType);

        if (cls != expectedCls)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int size = (int)H5AttributeNativeMethods.GetStorageSize(this);

        if (size != Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
        }

        unsafe
        {
            T result = default;
            int err = H5A.read(this, type, new IntPtr(&result));
            err.ThrowIfError("H5A.read");
            return result;
        }
    }

    public void Write(H5Type type, IntPtr buffer)
    {
        H5AttributeNativeMethods.Write(this, type, buffer);
    }

    #endregion
}
