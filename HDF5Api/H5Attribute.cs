using CommunityToolkit.HighPerformance.Buffers;
using HDF5Api.Disposables;
using System;
using System.Text;

namespace HDF5Api;

/// <summary>
///     .NET wrapper for the H5A (Attribute) API: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
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

    public H5Type GetH5Type()
    {
        return H5AttributeNativeMethods.GetType(this);
    }

    public string ReadString()
    {
        using var type = GetH5Type();
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

        Span<byte> buffer = stackalloc byte[(int)size];
        H5AttributeNativeMethods.Read(this, type, buffer);
        return Encoding.ASCII.GetString(buffer);
    }

    public DateTime ReadDateTime()
    {
        return DateTime.FromOADate(Read<double>());
    }

    public T Read<T>() where T : unmanaged
    {
        using var type = GetH5Type();
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

    // TODO: expose public Write<T> etc as per Read
    internal void Write(H5Type type, IntPtr buffer)
    {
        H5AttributeNativeMethods.Write(this, type, buffer);
    }

    #endregion
}
