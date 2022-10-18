using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5P (Property list) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
public abstract class H5PropertyList : H5Object<H5PropertyList>, IEquatable<H5PropertyList>
{
    internal protected H5PropertyList(long handle) : base(handle, HandleType.PropertyList, H5PAdapter.Close)
    {
    }

    public bool IsEqualTo([AllowNull] H5PropertyList? other)
    {
        if(other is null) { return false; }

        return H5PAdapter.AreEqual(this, other);
    }

#if NET7_0_OR_GREATER
    public bool Equals([AllowNull] H5PropertyList? other)
#else
    public bool Equals([AllowNull] H5PropertyList other)
#endif
    {
        return IsEqualTo(other);
    }

    // TODO: getclass, copy, ...
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

public class H5DataSetCreationPropertyList : H5PropertyList
{
    internal H5DataSetCreationPropertyList(long handle) : base(handle)
    {
    }

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
    public void SetDeflate(int level)
    {
        Guard.IsBetweenOrEqualTo(level, 0, 9);

        H5PAdapter.SetDeflate(this, level);
    }

    // TODO: GetChunk, etc.... loads
}

public class H5DataSetAccessPropertyList : H5PropertyList
{
    internal H5DataSetAccessPropertyList(long handle) : base(handle)
    {
    }
}

public class H5FileAccessPropertyList : H5PropertyList
{
    public H5FileAccessPropertyList(long handle) : base(handle)
    {
    }
}

public class H5FileCreationPropertyList : H5PropertyList
{
    internal H5FileCreationPropertyList(long handle) : base(handle)
    {
    }
}

public class H5GroupCreationPropertyList : H5PropertyList
{
    internal H5GroupCreationPropertyList(long handle) : base(handle)
    {
    }
}


public class H5TypeCreationPropertyList : H5PropertyList
{
    internal H5TypeCreationPropertyList(long handle) : base(handle)
    {
    }

}

public class H5TypeAccessPropertyList : H5PropertyList
{
    internal H5TypeAccessPropertyList(long handle) : base(handle)
    {
    }
}

// Etc...