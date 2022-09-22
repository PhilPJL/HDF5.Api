using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5T (Type) API.
/// </summary>
public struct H5Type : IDisposable
{
    #region Handle wrappers
    private long Handle { get; set; } = H5Handle.DefaultHandleValue;
    private bool IsNative { get; set; }

    internal H5Type(Handle handle, bool isNative = false)
    {
        Handle = handle;
        IsNative = isNative;
        if (!IsNative)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public void Dispose()
    {
        if (!IsNative && Handle > H5Handle.DefaultHandleValue)
        {
            H5TypeNativeMethods.Close(this);
            H5Handle.UntrackHandle(Handle);
        }

        Handle = H5Handle.DefaultHandleValue;
    }

    public static implicit operator long(H5Type h5object)
    {
        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }
    #endregion

    #region Public Api

    public static H5Type GetNativeType<T>() where T : unmanaged
    {
        long nativeHandle = default(T) switch
        {
            //            char => H5T.NATIVE_CHAR,

            short => H5T.NATIVE_INT16,
            ushort => H5T.NATIVE_USHORT,
            int => H5T.NATIVE_INT32,
            uint => H5T.NATIVE_UINT32,
            long => H5T.NATIVE_INT64,
            ulong => H5T.NATIVE_UINT64,
            float => H5T.NATIVE_FLOAT,
            double => H5T.NATIVE_DOUBLE,

            // TODO: add more mappings as required

            _ => throw new Hdf5Exception($"No mapping defined from {typeof(T).Name} to native type.")
        };

        return new H5Type(nativeHandle, true);
    }

    public H5Type Insert(string name, int offset, Handle nativeTypeId)
    {
        H5TypeNativeMethods.Insert(this, name, new IntPtr(offset), nativeTypeId);
        return this;
    }

    public H5Type Insert(string name, int offset, H5Type dataTypeId)
    {
        H5TypeNativeMethods.Insert(this, name, new IntPtr(offset), dataTypeId);
        return this;
    }

    public H5Type Insert(string name, IntPtr offset, Handle nativeTypeId)
    {
        H5TypeNativeMethods.Insert(this, name, offset, nativeTypeId);
        return this;
    }

    public H5Type Insert(string name, IntPtr offset, H5Type dataTypeId)
    {
        H5TypeNativeMethods.Insert(this, name, offset, dataTypeId);
        return this;
    }

    public H5Type Insert<S>(string name, Handle nativeTypeId) where S : struct
    {
        var offset = Marshal.OffsetOf<S>(name);
        H5TypeNativeMethods.Insert(this, name, offset, nativeTypeId);
        return this;
    }

    public H5Type Insert<S>(string name, H5Type dataTypeId) where S : struct
    {
        var offset = Marshal.OffsetOf<S>(name);
        H5TypeNativeMethods.Insert(this, name, offset, dataTypeId);
        return this;
    }

    public H5T.class_t GetClass()
    {
        return H5TypeNativeMethods.GetClass(this);
    }

    #endregion
}
