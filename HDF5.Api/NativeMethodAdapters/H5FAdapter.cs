using HDF5.Api.NativeMethods;
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
        int err = close(file);

        err.ThrowIfError();
    }

    internal static H5File Create(string path, bool failIfExists = false, H5PropertyList? fileCreationPropertyList = null, H5PropertyList? fileAccessPropertyList = null)
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

        h.ThrowIfInvalidHandleValue();

        return new H5File(h);
    }

    internal static void Flush(H5File file, bool flushGlobal)
    {
        int err = flush(file, flushGlobal ? scope_t.GLOBAL : scope_t.LOCAL);

        err.ThrowIfError();
    }

    internal static string GetName(H5File file)
    {
        return MarshalHelpers.GetName(file, get_name);
    }

    internal static long GetSize(H5File file)
    {
        ulong size = 0;
        int err = get_filesize(file, ref size);
        err.ThrowIfError();
        return (long)size;
    }

    internal static long GetObjectCount(H5File file, H5FObjectType types)
    {
#if NET7_0_OR_GREATER
        return get_obj_count(file, (uint)types);
#else
        return (long)get_obj_count(file, (uint)types);
#endif
    }

    [Obsolete]
    internal static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.FILE_CREATE),
            PropertyListType.Access => H5PAdapter.Create(H5P.FILE_ACCESS),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
    }

    [Obsolete]
    internal static H5PropertyList GetPropertyList(H5File file, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(file, get_create_plist),
            PropertyListType.Access => H5PAdapter.GetPropertyList(file, get_access_plist),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
    }

    internal static H5File Open(string path, bool readOnly, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = open(path, readOnly ? ACC_RDONLY : ACC_RDWR, fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5File(h);
    }
}
