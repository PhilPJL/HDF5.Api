namespace HDF5Api;

/// <summary>
///     Wrapper for H5P (Property list) API.
/// </summary>
public class H5PropertyList : H5Object<H5PropertyListHandle>
{
    private H5PropertyList(Handle handle) : base(new H5PropertyListHandle(handle)) { }

    public void SetChunk(int rank, ulong[] dims)
    {
        SetChunk(this, rank, dims);
    }

    /// <summary>
    ///     Level 0 = off
    ///     Level 1 = min compression + min CPU
    ///     ..
    ///     Level 9 = max compression + max CPU and time
    /// </summary>
    /// <param name="level"></param>
    public void EnableDeflateCompression(uint level)
    {
        EnableDeflateCompression(this, level);
    }

    #region C API wrappers

    public static H5PropertyList Create(Handle classId)
    {
        Handle h = H5P.create(classId);

        h.ThrowIfNotValid("H5P.create");

        return new H5PropertyList(h);
    }

    public static void SetChunk(H5PropertyListHandle propertyListId, int rank, ulong[] dims)
    {
        propertyListId.ThrowIfNotValid();

        int err = H5P.set_chunk(propertyListId, rank, dims);

        err.ThrowIfError("H5P.set_chunk");
    }

    public static void EnableDeflateCompression(H5PropertyListHandle propertyListId, uint level)
    {
        propertyListId.ThrowIfNotValid();

        int err = H5P.set_deflate(propertyListId, level);

        err.ThrowIfError("H5P.set_deflate");
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    public static H5PropertyList GetCreationPropertyList(H5DataSetHandle dataSet)
    {
        Handle h = H5D.get_create_plist(dataSet);
        h.ThrowIfNotValid("H5D.get_create_plist");
        return new H5PropertyList(h);
    }

    #endregion
}
