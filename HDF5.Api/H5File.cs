using CommunityToolkit.Diagnostics;
using System.IO;

using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5F (File) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_f.html"/>
/// </summary>
public class H5File : H5Location<H5File>
{
    internal H5File(long handle) : base(handle, HandleType.File, H5FAdapter.Close)
    {
    }

    public void Flush(bool flushGlobal = false)
    {
        H5FAdapter.Flush(this, flushGlobal);
    }

    public long GetObjectCount(H5ObjectTypes types = H5ObjectTypes.All)
    {
        return H5FAdapter.GetObjectCount(this, types);
    }

    public string Name => H5FAdapter.GetName(this);

    public long Size => H5FAdapter.GetSize(this);

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5FAdapter.CreatePropertyList(listType);
    }

    /// <summary>
    /// Gets a copy of the specified property list used to create the object
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5FAdapter.GetPropertyList(this, listType);
    }

    /// <summary>
    ///     Open an existing file.  By default opens read-write.
    /// </summary>
    public static H5File Open([DisallowNull] string path, bool readOnly = false, 
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FAdapter.Open(path, readOnly, fileAccessPropertyList);
    }

    /// <summary>
    ///     Open an existing file (by default read-write) or create new.
    /// </summary>
    public static H5File CreateOrOpen([DisallowNull] string path, bool readOnly = false,
        [AllowNull] H5PropertyList? fileCreationPropertyList = null,
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return File.Exists(path)
            ? H5FAdapter.Open(path, readOnly, fileAccessPropertyList)
            : H5FAdapter.Create(path, true, fileCreationPropertyList, fileAccessPropertyList);
    }

    /// <summary>
    /// Create a new H5 file
    /// </summary>
    /// <param name="path">Path of the file</param>
    public static H5File Create([DisallowNull] string path, 
        bool failIfExists = false, 
        [AllowNull] H5PropertyList? fileCreationPropertyList = null, 
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FAdapter.Create(path, failIfExists, fileCreationPropertyList, fileAccessPropertyList);
    }
}
