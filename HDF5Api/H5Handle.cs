global using Handle = System.Int64;
global using HDF.PInvoke;
using System;
using System.Diagnostics;

namespace HDF5Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Not entirely happy with this abstraction.  
    /// Could have H5Location base class deriving from Handle but that would mean removing all the Handle types
    /// and having File/Location/Handle, DataSet/Location/Handle etc.
    /// </remarks>
    public interface IH5Location
    {
        public Handle Handle { get; }

        public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        public H5Group CreateGroup(string name);
    }

    public class H5PropertyListHandle : H5Handle
    {
        public H5PropertyListHandle(Handle handle) : base(handle, H5P.close) { }
    }

    public class H5TypeHandle : H5Handle
    {
        public H5TypeHandle(Handle handle) : base(handle, H5T.close) { }
    }

    public abstract class H5FileHandle : H5Handle, IH5Location
    {
        protected H5FileHandle(Handle handle) : base(handle, H5F.close) { }

        public abstract H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        public abstract H5Group CreateGroup(string name);
    }

    public abstract class H5GroupHandle : H5Handle, IH5Location
    {
        protected H5GroupHandle(Handle handle) : base(handle, H5G.close) { }

        public abstract H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        public abstract H5Group CreateGroup(string name);
    }

    public class H5DataSetHandle : H5Handle
    {
        public H5DataSetHandle(Handle handle) : base(handle, H5D.close) { }
    }

    public class H5SpaceHandle : H5Handle
    {
        public H5SpaceHandle(Handle handle) : base(handle, H5S.close) { }
    }

    /// <summary>
    /// Base class for H5 handles.
    /// </summary>
    /// <remarks>
    /// Used to properly dispose H5 handles by calling the appropriate H5'X'.Close() method.
    /// </remarks>
    public abstract class H5Handle : Disposable
    {
        /// <summary>
        /// Func used to close the handle
        /// </summary>
        private Func<Handle, int> CloseHandle { get; }

        /// <summary>
        /// The int32/int64 handle returned by the H5 API
        /// </summary>
        public Handle Handle { get; private set; }

        /// <summary>
        /// The invalid handle value (there may be a value for this in P.Invoke)
        /// </summary>
        public const Handle InvalidHandle = -1;

        protected H5Handle(Handle handle, Func<Handle, int> closer)
        {
            Debug.WriteLine($"Handle opened {handle}: {GetType().Name}");

            AssertHandle(handle);
            Handle = handle;
            CloseHandle = closer;
        }

        public static void AssertHandle(Handle handle)
        {
            // TODO: give more information, specific exception
            if (handle <= 0)
            {
                throw new InvalidOperationException("Bad handle");
            }
        }

        public static void AssertError(int err)
        {
            // TODO: give more information, specific exception
            if (err < 0)
            {
                throw new InvalidOperationException("Error");
            }
        }

        public static implicit operator Handle(H5Handle h)
        {
            AssertHandle(h.Handle);
            return h.Handle;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                Debug.WriteLine($"Closing handle {Handle}");

                var err = CloseHandle(Handle);
                Handle = InvalidHandle;
                AssertError(err);
            }
        }
    }
}
