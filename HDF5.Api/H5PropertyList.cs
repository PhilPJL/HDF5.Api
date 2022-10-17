using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5P (Property list) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
public class H5PropertyList : H5Object<H5PropertyList>
{
    internal H5PropertyList(long handle) : base(handle, HandleType.PropertyList, H5PAdapter.Close)
    {
    }

    // TODO: move this to appropriate sub-class
    public void SetChunk([DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims, nameof(dims));
        Guard.IsGreaterThanOrEqualTo(1, dims.Length, nameof(dims));

        H5PAdapter.SetChunk(this, dims.Length, dims);
    }

    // TODO: move this to appropriate sub-class
    /// <summary>
    ///     Level 0 = off
    ///     Level 1 = min compression + min CPU
    ///     ..
    ///     Level 9 = max compression + max CPU and time
    /// </summary>
    /// <param name="level"></param>
    public void EnableDeflateCompression(int level)
    {
        Guard.IsBetweenOrEqualTo(level, 0, 9);

        H5PAdapter.EnableDeflateCompression(this, level);
    }

    public bool IsEqualTo([DisallowNull] H5PropertyList other)
    {
        Guard.IsNotNull(other);

        return H5PAdapter.AreEqual(this, other);
    }

    public static bool AreEqual([DisallowNull] H5PropertyList a, [DisallowNull] H5PropertyList b)
    {
        Guard.IsNotNull(a);
        Guard.IsNotNull(b);

        return a.Equals(b);
    }
}

internal class H5AttributeCreationPropertyList : H5PropertyList
{
    internal H5AttributeCreationPropertyList(long handle) : base(handle)
    {
    }

    public CharacterSet CharacterEncoding
    {
        get => H5PAdapter.GetCharacterEncoding(this);
        set => H5PAdapter.SetCharacterEncoding(this, value);
    }
}

internal class H5LinkCreationPropertyList : H5AttributeCreationPropertyList
{
    internal H5LinkCreationPropertyList(long handle) : base(handle)
    {
    }

    public bool CreateIntermediateGroups
    {
        get => H5PAdapter.GetCreateIntermediateGroups(this);
        set => H5PAdapter.SetCreateIntermediateGroups(this, value);
    }
}