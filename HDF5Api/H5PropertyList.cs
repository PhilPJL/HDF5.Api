using CommunityToolkit.Diagnostics;
using HDF5Api.NativeMethodAdapters;
namespace HDF5Api;

/// <summary>
///     Wrapper for H5P (Property list) API.
/// </summary>
public class H5PropertyList : H5Object<H5PropertyList>
{
    internal H5PropertyList(long handle) : base(handle, H5PAdapter.Close)
    {
    }
      
    // TODO: remove rank
    public void SetChunk(int rank, [DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims, nameof(dims));
        Guard.IsGreaterThanOrEqualTo(1, dims.Length, nameof(dims));

        H5PAdapter.SetChunk(this, rank, dims);
    }

    /// <summary>
    ///     Level 0 = off
    ///     Level 1 = min compression + min CPU
    ///     ..
    ///     Level 9 = max compression + max CPU and time
    /// </summary>
    /// <param name="level"></param>
    public void EnableDeflateCompression(int level)
    {
        H5PAdapter.EnableDeflateCompression(this, level);
    }
}

public enum PropertyListType
{
    Create,
    Access
        // Transfer?
}

