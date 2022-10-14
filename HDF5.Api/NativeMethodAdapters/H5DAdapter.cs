﻿#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5D;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 data-set native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
internal static class H5DAdapter
{
    internal static void Close(H5DataSet dataSet)
    {
        int err = close(dataSet);

        err.ThrowIfError();
    }

    internal static ulong GetStorageSize(H5DataSet dataSetId)
    {
        return get_storage_size(dataSetId);
    }

    internal static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5PropertyList? linkCreationPropertyList = null,
        H5PropertyList? dataSetCreationPropertyList = null,
        H5PropertyList? accessCreationPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        long h = create(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5DataSet(h);
    }

    public static IEnumerable<T> Read<T>(H5DataSet dataSet) 
        where T : unmanaged 
    {
        // TODO: move this check into separate method
        using var space = dataSet.GetSpace();
        using var type = dataSet.GetH5Type();
        long count = space.GetSimpleExtentNPoints();

        var cls = type.GetClass();

        if (cls != H5Class.Compound)
        {
            throw new Hdf5Exception($"DataSet is of class {cls} when expecting {H5Class.Compound}.");
        }

        long size = (long)GetStorageSize(dataSet);

        if (size != count * Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for {count} items of type {typeof(T).Name} of {count * Marshal.SizeOf<T>()}.");
        }

#if NET7_0_OR_GREATER
        if (size < 256)
        {
            Span<T> buf = stackalloc T[(int)count];
            read(dataSet, type, space, space, 0, MemoryMarshal.AsBytes(buf));
            return buf.ToArray();
        }
        else
        {
            using var buf = SpanOwner<T>.Allocate((int)count);
            read(dataSet, type, space, space, 0, MemoryMarshal.AsBytes(buf.Span));
            return buf.Span.ToArray();
        }
#else
        unsafe
        {
            var result = new T[count];
            fixed (T* ptr = result)
            {
                int err = H5D.read(dataSet, type, space, space, 0, new IntPtr(ptr));
                err.ThrowIfError();
                return result;
            }
        }
#endif
    }

    internal static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        long h = open(location, name, dataSetAccessPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5DataSet(h);
    }

    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        return H5LAdapter.Exists(location, name, linkAccessPropertyList);
    }

    internal static void SetExtent(H5DataSet dataSetId, params long[] dimensions)
    {
        int err = set_extent(dataSetId, dimensions.Select(d => (ulong)d).ToArray());

        err.ThrowIfError();
    }

    internal static void Write(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer, H5PropertyList? transferPropertyList = null) 
    {
        int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer);

        err.ThrowIfError();
    }

    // TODO:
    /*    public static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, Span<T> buffer, H5PropertyList? transferPropertyList = null) where T : unmanaged
        {
            int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, MemoryMarshal.Cast<T, byte>(buffer));

            err.ThrowIfError(nameof(write));
        }*/

    internal static H5Space GetSpace(H5DataSet dataSet)
    {
        long h = get_space(dataSet);

        h.ThrowIfInvalidHandleValue();

        return new H5Space(h);
    }

    internal static H5Type GetType(H5DataSet dataSet)
    {
        long h = get_type(dataSet);

        h.ThrowIfInvalidHandleValue();

        return new H5Type(h);
    }

    internal static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.DATASET_CREATE),
            PropertyListType.Access => H5PAdapter.Create(H5P.DATASET_ACCESS),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    internal static H5PropertyList GetPropertyList(H5DataSet dataSet, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(dataSet, get_create_plist),
            PropertyListType.Access => H5PAdapter.GetPropertyList(dataSet, get_access_plist),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
    }
}

