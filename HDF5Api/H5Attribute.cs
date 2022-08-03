using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HDF5Api.Disposables;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5G (Attribute) API.
/// </summary>
public class H5Attribute : H5Object<H5AttributeHandle>
{
    private H5Attribute(Handle handle) : base(new H5AttributeHandle(handle)) { }

    public void Write(H5TypeHandle typeId, IntPtr buffer)
    {
        Write(this, typeId, buffer);
    }

    public H5Type GetH5Type()
    {
        return H5Type.GetType(Handle);
    }

    public static Handle GetNativeType<T>() where T : unmanaged
    {
        return default(T) switch
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
        if (cls != H5T.class_t.STRING)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting STRING.");
        }

        long size = GetStorageSize(this);

        // TODO: probably simpler way to do this.
        using var buffer = new GlobalMemory((int)(size + 1));
        int err = H5A.read(Handle, type.Handle, buffer.IntPtr);
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
        using var type = GetH5Type();
        using var space = GetSpace();
        long count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();
        using var h = H5TypeHandle.WrapNative(GetNativeType<T>());
        var expectedCls = H5Type.GetClass(h);

        if (cls != expectedCls)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int size = (int)GetStorageSize(this);

        if (size != Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
        }

        unsafe
        {
            T result = default;
            int err = H5A.read(Handle, type.Handle, new IntPtr(&result));
            err.ThrowIfError("H5A.read");
            return result;
        }
    }

    public H5Space GetSpace()
    {
        return GetSpace(Handle);
    }

    #region C level API wrappers

    #region Create
    private static H5Attribute CreateImpl(H5Handle objectId, string name, H5TypeHandle typeId,
        H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
    {
        objectId.ThrowIfNotValid();
        typeId.ThrowIfNotValid();
        spaceId.ThrowIfNotValid();
        propertyListId.ThrowIfNotValid();

        Handle h = H5A.create(objectId.Handle, name, typeId, spaceId, propertyListId);

        h.ThrowIfNotValid("H5A.create");

        return new H5Attribute(h);
    }

    public static H5Attribute Create(H5DataSetHandle dataSetId, string name, H5TypeHandle typeId,
        H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
    {
        return CreateImpl(dataSetId, name, typeId, spaceId, propertyListId);
    }

    public static H5Attribute Create(H5LocationHandle locationId, string name, H5TypeHandle typeId,
        H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
    {
        return CreateImpl(locationId, name, typeId, spaceId, propertyListId);
    }
    #endregion

    #region Open
    private static H5Attribute OpenAttributeImpl(H5Handle attributeParentId, string name)
    {
        attributeParentId.ThrowIfNotValid();

        Handle h = H5A.open(attributeParentId.Handle, name);

        h.ThrowIfNotValid("H5A.open");

        return new H5Attribute(h);
    }

    public static H5Attribute Open(H5LocationHandle locationId, string name)
    {
        return OpenAttributeImpl(locationId, name);
    }

    public static H5Attribute Open(H5DataSetHandle dataSetId, string name)
    {
        return OpenAttributeImpl(dataSetId, name);
    }
    #endregion

    #region Delete
    private static void DeleteImpl(H5Handle objectId, string name)
    {
        int err = H5A.delete(objectId, name);

        err.ThrowIfError("H5A.delete");
    }

    public static void Delete(H5LocationHandle locationId, string name)
    {
        DeleteImpl(locationId, name);
    }

    public static void Delete(H5DataSetHandle dataSetId, string name)
    {
        DeleteImpl(dataSetId, name);
    }
    #endregion

    #region Exists
    private static bool ExistsImpl(H5Handle objectId, string name)
    {
        objectId.ThrowIfNotValid();
        int err = H5A.exists(objectId, name);
        err.ThrowIfError("H5A.exists");
        return err > 0;
    }

    public static bool Exists(H5LocationHandle locationId, string name)
    {
        return ExistsImpl(locationId, name);
    }

    public static bool Exists(H5DataSetHandle dataSetId, string name)
    {
        return ExistsImpl(dataSetId, name);
    }
    #endregion

    #region List attribute names
    private static IEnumerable<string> ListAttributeNamesImpl(H5Handle handle)
    {
        handle.ThrowIfNotValid();

        if(handle is not H5DataSetHandle && handle is not H5GroupHandle && handle is not H5FileHandle)
        {
            throw new Hdf5Exception($"The supplied handle is of type {handle.GetType().Name}.  It must be H5DataSetHandle, H5GroupHandle or H5FileHandle.");
        }

        ulong idx = 0;

        var names = new List<string>();
        
        int err = H5A.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError("H5A.iterate");

        return names;

        int Callback(Handle id, IntPtr intPtrName, ref H5A.info_t info, IntPtr _)
        {
            string name = Marshal.PtrToStringAnsi(intPtrName);

            int err1 = H5A.get_info_by_name(handle, ".", name, ref info);
            err1.ThrowIfError("H5A.get_info_by_name");

            Debug.WriteLine($"{name}: {info.data_size}");

            names.Add(name);
            return 0;
        }
    }

    public static IEnumerable<string> ListAttributeNames(H5LocationHandle locationId)
    {
        return ListAttributeNamesImpl(locationId);
    }

    public static IEnumerable<string> ListAttributeNames(H5DataSetHandle dataSetId)
    {
        return ListAttributeNamesImpl(dataSetId);
    }
    #endregion

    public static void Write(H5AttributeHandle attributeId, H5TypeHandle typeId, IntPtr buffer)
    {
        int err = H5A.write(attributeId, typeId, buffer);

        err.ThrowIfError("H5A.write");
    }

    public static H5Space GetSpace(H5AttributeHandle attributeId)
    {
        attributeId.ThrowIfNotValid();
        Handle h = H5A.get_space(attributeId.Handle);
        h.ThrowIfNotValid("H5A.get_space");
        return new H5Space(h);
    }

    public static long GetStorageSize(H5AttributeHandle attributeId)
    {
        attributeId.ThrowIfNotValid();
        return (long)H5A.get_storage_size(attributeId.Handle);
    }

    #endregion
}
