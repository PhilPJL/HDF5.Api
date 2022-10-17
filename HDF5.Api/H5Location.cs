using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     Intermediate base class for H5File and H5Group.
/// </summary>
/// <remarks>
///     Implements operations that can be carried out equally on files and groups.
/// </remarks>
public abstract class H5Location<T> : H5Object<T>, IH5Location where T : H5Object<T>
{
    internal H5Location(long handle, HandleType handleType, Action<T>? closeHandle)
        : base(handle, handleType, closeHandle)
    {
    }

    /// <summary>
    ///     Create an Attribute for this location
    /// </summary>
    public H5Attribute CreateAttribute(
        [DisallowNull] string name, 
        [DisallowNull] H5Type type, 
        [DisallowNull] H5Space space)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5AAdapter.Create(this, name, type, space);
    }

    public H5Attribute CreateStringAttribute(
        [DisallowNull] string name, 
        int fixedStorageLength = 0, CharacterSet characterSet = CharacterSet.Ascii, StringPadding padding = StringPadding.NullTerminate)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding);
    }

    /// <summary>
    ///     Open an existing Attribute for this location
    /// </summary>
    public H5Attribute OpenAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name);
    }

    public void DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        H5AAdapter.Delete(this, name);
    }

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public TA ReadAttribute<TA>([DisallowNull] string name) where TA : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.Read<TA>(attribute);
    }

    public string ReadStringAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadString(attribute);
    }

    public DateTime ReadDateTimeAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadDateTime(attribute);
    }

    public IEnumerable<string> AttributeNames => H5AAdapter.GetAttributeNames(this);
    public IEnumerable<string> GroupNames => H5LAdapter.GetGroupNames(this);
    public IEnumerable<string> DataSetNames => H5LAdapter.GetDataSetNames(this);
    public IEnumerable<string> NamedDataTypeNames => H5LAdapter.GetNamedDataTypeNames(this);
    public IEnumerable<(string name, H5ObjectType type)> Members => H5LAdapter.GetMembers(this);

    public int NumberOfAttributes => (int)H5OAdapter.GetInfo(this).num_attrs;

    /// <summary>
    ///     Create a Group in this location
    /// </summary>
    public H5Group CreateGroup([DisallowNull] string name,
        [AllowNull] H5PropertyList? linkCreationPropertyList = null,
        [AllowNull] H5PropertyList? groupCreationPropertyList = null,
        [AllowNull] H5PropertyList? groupAccessPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Create(this, name, linkCreationPropertyList, groupCreationPropertyList, groupAccessPropertyList);
    }

    /// <summary>
    ///     Open an existing group in this location
    /// </summary>
    public H5Group OpenGroup([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Open(this, name);
    }

    public bool GroupExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Exists(this, name);
    }

    public bool GroupPathExists([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5GAdapter.PathExists(this, path);
    }

    public void DeleteGroup([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        H5GAdapter.Delete(this, path);
    }

    /// <summary>
    ///     Create a DataSet in this location
    /// </summary>
    public H5DataSet CreateDataSet([DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] H5Space space, [AllowNull] H5PropertyList? dataSetCreationPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5DAdapter.Create(this, name, type, space, null, dataSetCreationPropertyList);
    }

    /// <summary>
    ///     Open an existing DataSet in this location
    /// </summary>
    public H5DataSet OpenDataSet(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5DAdapter.Open(this, name);
    }

    public bool DataSetExists(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5DAdapter.Exists(this, name);
    }

    public void Commit(
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type,
        [AllowNull] H5PropertyList? linkCreationPropertyList = null,
        [AllowNull] H5PropertyList? dataTypeCreationPropertyList = null,
        [AllowNull] H5PropertyList? dataTypeAccessPropertyList = null)
    {
        H5TAdapter.Commit(this, name, h5Type,
            linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList);
    }
}
