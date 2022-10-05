using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

/// <summary>
///     .NET wrapper for the H5D (DataSet) API: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
public class H5DataSet : H5Object<H5DataSet>, IH5ObjectWithAttributes
{
    internal H5DataSet(long handle) : base(handle, H5DAdapter.Close)
    {
    }

    public void Write([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, [DisallowNull] IntPtr buffer) 
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(memorySpace);
        Guard.IsNotNull(fileSpace);
        Guard.IsNotNull(buffer);

        H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer);
    }

/*    public void Write([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, [DisallowNull] T[] buffer) where T : unmanaged
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(memorySpace);
        Guard.IsNotNull(fileSpace);
        Guard.IsNotNull(buffer);

        H5DAdapter.Write<T>(this, type, memorySpace, fileSpace, buffer);
    }
*/
/*    public void Write<T>([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, Span<T> buffer) where T : unmanaged
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(memorySpace);
        Guard.IsNotNull(fileSpace);

        H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer);
    }*/

    public H5Space GetSpace()
    {
        return H5DAdapter.GetSpace(this);
    }

    public void SetExtent([DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims);

        H5DAdapter.SetExtent(this, dims);
    }

    public H5Type GetH5Type()
    {
        return H5DAdapter.GetType(this);
    }

    /// <summary>
    ///     Open an existing Attribute for this dataset
    /// </summary>
    public H5Attribute OpenAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name);
    }

    public H5Attribute CreateAttribute([DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] H5Space space, H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5AAdapter.Create(this, name, type, space, creationPropertyList);
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

    public T ReadAttribute<T>([DisallowNull] string name) where T : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return this.ReadAttribute<H5DataSet, T>(name);
    }

    public string ReadStringAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5ObjectWithAttributeExtensions.ReadStringAttribute(this, name);
    }

    public DateTime ReadDateTimeAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5ObjectWithAttributeExtensions.ReadDateTimeAttribute(this, name);
    }

    public IEnumerable<string> ListAttributeNames()
    {
        return H5AAdapter.ListAttributeNames(this);
    }

    public static H5PropertyList CreatePropertyList(PropertyList listType)
    {
        return H5DAdapter.CreatePropertyList(listType);
    }

    public H5PropertyList GetPropertyList(PropertyList listType)
    {
        return H5DAdapter.GetPropertyList(this, listType);
    }

    public IEnumerable<T> Read<T>() where T : unmanaged
    {
        // TODO: move this check into separate method
        using var space = GetSpace();
        using var type = GetH5Type();
        long count = space.GetSimpleExtentNPoints();

        var cls = type.GetClass();
        if (cls != H5Class.Compound)
        {
            throw new Hdf5Exception($"DataSet is of class {cls} when expecting {H5Class.Compound}.");
        }

        long size = (long)H5DAdapter.GetStorageSize(this);

        if (size != count * Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for {count} items of type {typeof(T).Name} of {count * Marshal.SizeOf<T>()}.");
        }

        // TODO: move into adapter
        // TODO: create .NET7 Span<T> variant
        unsafe
        {
            var result = new T[count];
            fixed (T* ptr = result)
            {
                // TODO: use native H5DDataSetNativeMethods
                int err = NativeMethods.H5D.read(this, type, space, space, 0, new IntPtr(ptr));
                err.ThrowIfError(nameof(NativeMethods.H5D.read));
                return result;
            }
        }
    }
}