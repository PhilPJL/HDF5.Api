using CommunityToolkit.Diagnostics;
using HDF5Api.NativeMethodAdapters;
namespace HDF5Api;

/// <summary>
///     <para>.NET wrapper for H5P (Property list) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
public class H5PropertyList : H5Object<H5PropertyList>
{
    internal H5PropertyList(long handle) : base(handle, H5PAdapter.Close)
    {
    }
      
    // TODO: remove rank
    public void SetChunk([DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims, nameof(dims));
        Guard.IsGreaterThanOrEqualTo(1, dims.Length, nameof(dims));

        H5PAdapter.SetChunk(this, dims.Length, dims);
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

    public bool IsEqualTo(H5PropertyList other)
    {
        return H5PAdapter.AreEqual(this, other);
    }
}

public enum PropertyListType
{
    None = 0,
    Create,
    Access
}

