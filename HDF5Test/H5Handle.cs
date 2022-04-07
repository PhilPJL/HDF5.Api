using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public interface IH5Location
    {
        public Handle Handle { get; }
    }

    public class H5PropertyListHandle : H5Handle
    {
        public H5PropertyListHandle(Handle handle) : base(handle, H5P.close) { }
    }

    public class H5TypeHandle : H5Handle
    {
        public H5TypeHandle(Handle handle) : base(handle, H5T.close) { }
    }

    public class H5FileHandle : H5Handle, IH5Location
    {
        public H5FileHandle(Handle handle) : base(handle, H5F.close) { }
    }

    public class H5GroupHandle : H5Handle, IH5Location
    {
        public H5GroupHandle(Handle handle) : base(handle, H5G.close) { }
    }

    public class H5DataSetHandle : H5Handle
    {
        public H5DataSetHandle(Handle handle) : base(handle, H5D.close) { }
    }

    public class H5SpaceHandle : H5Handle
    {
        public H5SpaceHandle(Handle handle) : base(handle, H5S.close) { }
    }

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
                int err = CloseHandle(Handle);
                Handle = InvalidHandle;
                AssertError(err);
            }
        }
    }
}
