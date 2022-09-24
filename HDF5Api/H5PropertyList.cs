namespace HDF5Api;

/// <summary>
///     Wrapper for H5P (Property list) API.
/// </summary>
public class H5PropertyList : H5Object<H5PropertyList>
{
    internal H5PropertyList(long handle) : base(handle, H5PropertyListNativeMethods.Close)
    {
    }

    public void SetChunk(int rank, ulong[] dims)
    {
        H5PropertyListNativeMethods.SetChunk(this, rank, dims);
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
        H5PropertyListNativeMethods.EnableDeflateCompression(this, level);
    }
}

