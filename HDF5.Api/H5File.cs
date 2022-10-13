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

    /// <summary>
    /// Flush the file to disk.
    /// </summary>
    /// <param name="flushGlobal"></param>
    public void Flush(bool flushGlobal = false)
    {
        H5FAdapter.Flush(this, flushGlobal);
    }

    /// <summary>
    /// Returns the number of open objects by type.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public long GetObjectCount(H5FObjectType types = H5FObjectType.All)
    {
        return H5FAdapter.GetObjectCount(this, types);
    }

    /// <summary>
    /// The filename.
    /// </summary>
    public string Name => H5FAdapter.GetName(this);

    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    public long Size => H5FAdapter.GetSize(this);

    /// <summary>
    /// Creates a <see cref="H5PropertyList"/> of the required type.
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5FAdapter.CreatePropertyList(listType);
    }

    /// <summary>
    /// Gets a copy of the specified <see cref="H5PropertyList"/> used to create the object.
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5FAdapter.GetPropertyList(this, listType);
    }

    /// <summary>
    ///     Opens an existing file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="readOnly">Open the file in read-only mode.  Defaults to read-write.</param>
    public static H5File Open(
        [DisallowNull] string path, 
        bool readOnly = false, 
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FAdapter.Open(path, readOnly, fileAccessPropertyList);
    }

    /// <summary>
    ///     Opens an existing file or creates a new one if the file does not exist.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="readOnly">Open the file in read-only mode.  Defaults to read-write.</param>
    public static H5File CreateOrOpen(
        [DisallowNull] string path, 
        bool readOnly = false,
        [AllowNull] H5PropertyList? fileCreationPropertyList = null,
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return File.Exists(path)
            ? H5FAdapter.Open(path, readOnly, fileAccessPropertyList)
            : H5FAdapter.Create(path, true, fileCreationPropertyList, fileAccessPropertyList);
    }

    /// <summary>
    /// Attempts to create a new <see cref="H5File"/>.  
    /// By default an existing file will be truncated.
    /// </summary>
    /// <param name="path">Path tp the file</param>
    /// <param name="failIfExists">Fail if the file being created already exists.</param>
    public static H5File Create(
        [DisallowNull] string path, 
        bool failIfExists = false, 
        [AllowNull] H5PropertyList? fileCreationPropertyList = null, 
        [AllowNull] H5PropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5FAdapter.Create(path, failIfExists, fileCreationPropertyList, fileAccessPropertyList);
    }
}
