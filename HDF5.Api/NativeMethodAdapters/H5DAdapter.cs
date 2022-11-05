#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
using HDF5.Api.H5Types;
#endif
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5D;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 data-set native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
internal static unsafe class H5DAdapter
{
    internal static void Close(H5DataSet dataSet)
    {
        close(dataSet).ThrowIfError();
    }

    internal static ulong GetStorageSize(H5DataSet dataSetId)
    {
        return get_storage_size(dataSetId);
    }

    internal static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5DataSetCreationPropertyList? dataSetCreationPropertyList,
        H5DataSetAccessPropertyList? dataSetAccessPropertyList) where T : H5Location<T>
    {
        location.AssertHasLocationHandleType();

        using var linkCreationPropertyList = H5Link.CreateCreationPropertyList();

        long h = 0;

#if NET7_0_OR_GREATER
        h = create(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, dataSetAccessPropertyList);
#else
        fixed (byte* namePtr = Encoding.UTF8.GetBytes(name))
        {
            h = create(location, namePtr, type, space,
                linkCreationPropertyList, dataSetCreationPropertyList, dataSetAccessPropertyList);
        }
#endif

        return new H5DataSet(h);
    }

    public static IEnumerable<T> Read<T>(H5DataSet dataSet) 
        where T : unmanaged 
    {
        using var space = dataSet.GetSpace();
        using var type = dataSet.GetH5Type();
        long count = space.GetSimpleExtentNPoints();

        var cls = type.GetClass();

        if (cls != DataTypeClass.Compound)
        {
            throw new H5Exception($"DataSet is of class {cls} when expecting {DataTypeClass.Compound}.");
        }

        long size = (long)GetStorageSize(dataSet);

        if (size != count * sizeof(T))
        {
            throw new H5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for {count} items of type {typeof(T).Name} of {count * sizeof(T)}.");
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
        var value = new T[count];
        fixed (T* ptr = value)
        {
            int result = read(dataSet, type, space, space, 0, new IntPtr(ptr));
            result.ThrowIfError();
            return value;
        }
#endif
    }

    internal static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList) where T : H5Location<T>
    {
        location.AssertHasLocationHandleType();

        long h = 0;

#if NET7_0_OR_GREATER
        h = open(location, name, dataSetAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(location, nameBytesPtr, dataSetAccessPropertyList);
        }
#endif

        return new H5DataSet(h);
    }

    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList) where T : H5Location<T>
    {
        return H5LAdapter.Exists(location, name, linkAccessPropertyList);
    }

    internal static void SetExtent(H5DataSet dataSetId, params long[] dimensions)
    {
        set_extent(dataSetId, dimensions.Select(d => (ulong)d).ToArray()).ThrowIfError();
    }

    internal static void Write(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer, H5PropertyList? transferPropertyList) 
    {
        write(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer).ThrowIfError();
    }

#if NET7_0_OR_GREATER
    internal static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, Span<byte> buffer, H5PropertyList? transferPropertyList) 
    {
        write(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer).ThrowIfError(nameof(write));
    }
#endif

    internal static H5Space GetSpace(H5DataSet dataSet)
    {
        return new H5Space(get_space(dataSet));
    }

    internal static H5Type GetType(H5DataSet dataSet)
    {
        return new H5Type(get_type(dataSet));
    }
 
    internal static H5DataSetCreationPropertyList CreateCreationPropertyList()
    {
        return H5PAdapter.Create(H5P.DATASET_CREATE, h => new H5DataSetCreationPropertyList(h));
    }
 
    internal static H5DataSetCreationPropertyList GetCreationPropertyList(H5DataSet dataSet)
    {
        return H5PAdapter.GetPropertyList(dataSet, get_create_plist, h => new H5DataSetCreationPropertyList(h));
    }
 
    internal static H5DataSetAccessPropertyList CreateAccessPropertyList()
    {
        return H5PAdapter.Create(H5P.DATASET_ACCESS, h => new H5DataSetAccessPropertyList(h));
    }
 
    internal static H5DataSetAccessPropertyList GetAccessPropertyList(H5DataSet dataSet)
    {
        return H5PAdapter.GetPropertyList(dataSet, get_access_plist, h => new H5DataSetAccessPropertyList(h));
    }

    internal static void ReclaimVariableLengthMemory(H5Type type, H5Space space, H5PropertyList? propertyList, byte* buffer)
    {
        vlen_reclaim(type, space, propertyList, buffer).ThrowIfError();
    }

    internal static void ReclaimVariableLengthMemory(H5Type type, H5Space space, byte** buffer)
    {
        vlen_reclaim(type, space, H5P.DEFAULT, buffer).ThrowIfError();
    }

    internal static long GetVariableLengthBufferSize(H5DataSet dataSet, H5Type type, H5Space space)
    {
        ulong size = default;
        int result = vlen_get_buf_size(dataSet, type, space, ref size);
        result.ThrowIfError();
        return (long)size;
    }
}

