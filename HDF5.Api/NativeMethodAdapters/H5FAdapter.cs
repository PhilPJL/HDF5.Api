using HDF5.Api.NativeMethods;
using System.IO;
using static HDF5.Api.NativeMethods.H5F;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 file native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_f.html"/>
/// </summary>
internal static class H5FAdapter
{
    public static void Close(H5File file)
    {
        int err = close(file);

        err.ThrowIfError();
    }

    public static H5File Create(string path, bool failIfExists = false, H5PropertyList? fileCreationPropertyList = null, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = create(path, failIfExists ? ACC_EXCL : ACC_TRUNC, fileCreationPropertyList, fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5File(h);
    }

    public static void Flush(H5File file, bool flushGlobal)
    {
        int err = flush(file, flushGlobal ? scope_t.GLOBAL : scope_t.LOCAL);

        err.ThrowIfError();
    }

    public static string GetName(H5File file)
    {
#if NET7_0_OR_GREATER
        int length = (int)get_name(file, new Span<byte>(), 0);
        length.ThrowIfError();
        if (length > 261)
        {
            throw new PathTooLongException();
        }
        if (length <= 1)
        {
            throw new Hdf5Exception("Name length returned was too short");
        }
        Span<byte> buffer = stackalloc byte[length + 1];
        var err = (int)get_name(file, buffer, length + 1);
        err.ThrowIfError();
        // remove trailing \0
        return Encoding.ASCII.GetString(buffer.Slice(0, length));
#else
        int length = (int)get_name(file, null!, new IntPtr(0));
        length.ThrowIfError();
        if (length > 261)
        {
            throw new PathTooLongException();
        }
        if (length <= 1)
        {
            throw new Hdf5Exception("Name length returned was too short");
        }
        var name = new StringBuilder(length + 1);
        var err = get_name(file, name, new IntPtr(length + 1));
        ((int)err).ThrowIfError();
        return name.ToString();
#endif
    }

    public static long GetSize(H5File file)
    {
        ulong size = 0;
        int err = get_filesize(file, ref size);
        err.ThrowIfError();
        return (long)size;
    }

    public static long GetObjectCount(H5File file, H5ObjectTypes types = H5ObjectTypes.All)
    {
#if NET7_0_OR_GREATER
        return get_obj_count(file, (uint)types);
#else
        return (long)get_obj_count(file, (uint)types);
#endif
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.FILE_CREATE),
            PropertyListType.Access => H5PAdapter.Create(H5P.FILE_ACCESS),
            _ => throw new NotImplementedException(),
        };
    }

    public static H5PropertyList GetPropertyList(H5File file, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(file, get_create_plist),
            PropertyListType.Access => H5PAdapter.GetPropertyList(file, get_access_plist),
            _ => throw new NotImplementedException(),
        };
    }

    public static H5File Open(string path, bool readOnly, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = open(path, readOnly ? ACC_RDONLY : ACC_RDWR, fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5File(h);
    }
}
