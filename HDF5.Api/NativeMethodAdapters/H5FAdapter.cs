﻿using HDF5.Api.NativeMethods;
using HDF5.Api.Utils;
using static HDF5.Api.NativeMethods.H5F;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 file native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_f.html"/>
/// </summary>
internal static unsafe class H5FAdapter
{
    internal static void Close(H5File file)
    {
        close(file).ThrowIfError();
    }

    internal static H5File Create(string path, bool failIfExists = false,
        H5FileCreationPropertyList? fileCreationPropertyList = null, H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        long h;

#if NET7_0_OR_GREATER
        h = create(path, failIfExists ? ACC_EXCL : ACC_TRUNC, fileCreationPropertyList, fileAccessPropertyList);
#else
        fixed (byte* pathBytesPtr = Encoding.UTF8.GetBytes(path))
        {
            h = create(pathBytesPtr, failIfExists ? ACC_EXCL : ACC_TRUNC, fileCreationPropertyList, fileAccessPropertyList);
        }
#endif

        return new H5File(h);
    }

    internal static void Flush(H5File file, bool flushGlobal)
    {
        flush(file, flushGlobal ? scope_t.GLOBAL : scope_t.LOCAL).ThrowIfError();
    }

    internal static string GetName(H5File file)
    {
        return MarshalHelpers.GetName(file, get_name);
    }

    internal static long GetSize(H5File file)
    {
        ulong size = 0;
        get_filesize(file, ref size).ThrowIfError();
        return (long)size;
    }

    internal static long GetObjectCount(H5File file, FileObjectType types)
    {
#if NET7_0_OR_GREATER
        return get_obj_count(file, (uint)types);
#else
        return (long)get_obj_count(file, (uint)types);
#endif
    }

    internal static H5FileCreationPropertyList CreateCreationPropertyList()
    {
        return H5PAdapter.Create(H5P.FILE_CREATE, h => new H5FileCreationPropertyList(h));
    }

    internal static H5FileAccessPropertyList CreateAccessPropertyList()
    {
        return H5PAdapter.Create(H5P.FILE_ACCESS, h => new H5FileAccessPropertyList(h));
    }

    internal static H5FileCreationPropertyList GetCreationPropertyList(H5File file)
    {
        return H5PAdapter.GetPropertyList(file, get_create_plist, h => new H5FileCreationPropertyList(h));
    }

    internal static H5FileAccessPropertyList GetAccessPropertyList(H5File file)
    {
        return H5PAdapter.GetPropertyList(file, get_access_plist, h => new H5FileAccessPropertyList(h));
    }

    internal static H5File Open(string path, bool readOnly, H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        return new H5File(open(path, readOnly ? ACC_RDONLY : ACC_RDWR, fileAccessPropertyList));
    }

    // NOTE: get_libver_bounds isn't available - use file access property list instead
    internal static void SetLibraryVersionBounds(H5File file, LibraryVersion low, LibraryVersion high)
    {
        set_libver_bounds(file, (libver_t)low, (libver_t)high).ThrowIfError();
    }
}
