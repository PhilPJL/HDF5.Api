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
public abstract class H5Location<T> : H5ObjectWithAttributes<T> where T : H5Location<T>
{
    internal H5Location(long handle, HandleType handleType, Action<T>? closeHandle)
        : base(handle, handleType, closeHandle)
    {
    }

    public IEnumerable<string> GroupNames => H5LAdapter.GetGroupNames(this);

    public T Enumerate(Action<H5Group> action)
    {
        foreach (var name in GroupNames)
        {
            using var h5Object = OpenGroup(name);
            action(h5Object);
        }

        return (T)this;
    }

    public IEnumerable<string> DataSetNames => H5LAdapter.GetDataSetNames(this);

    public T Enumerate(Action<H5DataSet> action)
    {
        foreach (var name in DataSetNames)
        {
            using var h5Object = OpenDataSet(name);
            action(h5Object);
        }

        return (T)this;
    }

    public IEnumerable<string> DataTypeNames => H5LAdapter.GetNamedDataTypeNames(this);

    public T Enumerate(Action<H5Type> action)
    {
        foreach (var name in DataTypeNames)
        {
            using var h5Object = OpenType(name);
            action(h5Object);
        }

        return (T)this;
    }

    public IEnumerable<(string name, ObjectType type)> Members => H5LAdapter.GetMembers(this);

    /// <summary>
    ///     Create a Group in this location
    /// </summary>
    public H5Group CreateGroup([DisallowNull] string name)
    {
        return CreateGroup(name, null);
    }

    /// <summary>
    ///     Create a Group in this location
    /// </summary>
    internal H5Group CreateGroup([DisallowNull] string name,
        [AllowNull] H5GroupCreationPropertyList? groupCreationPropertyList)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Create(this, name, groupCreationPropertyList);
    }

    /// <summary>
    ///     Open an existing group in this location
    /// </summary>
    public H5Group OpenGroup([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Open(this, name, null);
    }

    public bool GroupExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5GAdapter.Exists(this, name, null);
    }

    public bool GroupPathExists([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        return H5GAdapter.PathExists(this, path, null);
    }

    public H5Location<T> DeleteGroup([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        H5LAdapter.Delete(this, path, null);
        return this;
    }

    /// <summary>
    ///     Create a DataSet in this location
    /// </summary>
    public H5DataSet CreateDataSet(
        [DisallowNull] string name,
        [DisallowNull] H5Type type,
        [DisallowNull] H5Space space,
        [AllowNull] H5DataSetCreationPropertyList? dataSetCreationPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return CreateDataSet(name, type, space, dataSetCreationPropertyList, null);
    }

    /// <summary>
    ///     Create a DataSet in this location
    /// </summary>
    internal H5DataSet CreateDataSet(
        [DisallowNull] string name,
        [DisallowNull] H5Type type,
        [DisallowNull] H5Space space,
        [AllowNull] H5DataSetCreationPropertyList? dataSetCreationPropertyList,
        [AllowNull] H5DataSetAccessPropertyList? dataSetAccessPropertyList)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5DAdapter.Create(this, name, type, space, dataSetCreationPropertyList, dataSetAccessPropertyList);
    }

    /// <summary>
    ///     Open an existing DataSet in this location
    /// </summary>
    public H5DataSet OpenDataSet(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5DAdapter.Open(this, name, null);
    }

    public bool DataSetExists(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5DAdapter.Exists(this, name, null);
    }

    public T DeleteDataSet([DisallowNull] string path)
    {
        Guard.IsNotNullOrWhiteSpace(path);

        H5LAdapter.Delete(this, path, null);

        return (T)this;
    }

    public T Commit([DisallowNull] string name, [DisallowNull] H5Type h5Type)
    {
        H5TAdapter.Commit(this, name, h5Type, null, null);

        return (T)this;
    }

    public H5Type OpenType(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5TAdapter.Open(this, name, null);
    }

    public abstract string Name { get; }
}
