using HDF5Api.Disposables;
using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5A (Attribute) API.
/// </summary>
public class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, H5AttributeNativeMethods.Close)
    {
    }

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
        if (cls != H5Class.String)
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
