using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    /// <summary>
    /// Operations that can be carried out on a location (group or file)
    /// </summary>
    /// <remarks>
    /// Add more methods as required.
    /// </remarks>
    public interface IH5Location
    {
        H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        H5Attribute OpenAttribute(string name);

        H5Group CreateGroup(string name);
        H5Group OpenGroup(string name);

        H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        H5DataSet OpenDataSet(string name);

        IEnumerable<string> GetChildNames();
    }

    /// <summary>
    /// Intermediate base class for H5File and H5Group.
    /// </summary>
    /// <remarks>
    /// Implements operations that can be carried out equally on files and groups.
    /// </remarks>
    /// <typeparam name="THandle"></typeparam>
    public abstract class H5Location<THandle> : H5Object<THandle>, IH5Location where THandle : H5LocationHandle
    {
        protected H5Location(THandle handle) : base(handle) { }

        /// <summary>
        /// Create an Attribute for this location
        /// </summary>
        public H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5Attribute.Create(Handle, name, typeId, spaceId, propertyListId);
        }

        /// <summary>
        /// Open an existing Attribute for this location
        /// </summary>
        public H5Attribute OpenAttribute(string name)
        {
            return H5Attribute.Open(Handle, name);
        }

        /// <summary>
        /// Create a Group in this location
        /// </summary>
        public H5Group CreateGroup(string name)
        {
            return H5Group.Create(Handle, name);
        }

        /// <summary>
        /// Open an existing group in this location
        /// </summary>
        public H5Group OpenGroup(string name)
        {
            return H5Group.Open(Handle, name);
        }

        /// <summary>
        /// Create a DataSet in this location
        /// </summary>
        public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        /// <summary>
        /// Open an existing DataSet in this location
        /// </summary>
        public H5DataSet OpenDataSet(string name)
        {
            return H5DataSet.Open(this, name);
        }

        /// <summary>
        /// Enumerate the names of child objects (groups, data-sets) in this location
        /// </summary>
        /// <remarks>
        /// TODO: return type also?
        /// </remarks>
        public IEnumerable<string> GetChildNames()
        {
            return GetChildNames(Handle);
        }

        private static IEnumerable<string> GetChildNames(H5LocationHandle handle)
        {
            handle.ThrowIfNotValid();

            ulong idx = 0;

            var names = new List<string>();

            int err = H5L.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
            err.ThrowIfError("H5L.iterate");

            return names;

            int Callback(Handle group, IntPtr intPtrName, ref H5L.info_t info, IntPtr _)
            {
                names.Add(Marshal.PtrToStringAnsi(intPtrName));
                return 0;
            }
        }
    }
}
