global using HDF.PInvoke;
global using Handle = System.Int64;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HDF5Api
{
    public class H5Object<THandle> : Disposable where THandle : H5Handle
    {
        public H5Object(THandle handle)
        {
            Handle = handle;
        }

        public THandle Handle { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !Handle.IsDisposed)
            {
                Handle.Dispose();
            }
        }

        public static implicit operator THandle (H5Object<THandle> h5Object)
        {
            return h5Object.Handle;
        }
    }

    public class H5AttributeHandle : H5Handle
    {
        public H5AttributeHandle(Handle handle) : base(handle, H5A.close) { }
    }

    public class H5PropertyListHandle : H5Handle
    {
        public H5PropertyListHandle(Handle handle) : base(handle, H5P.close) { }
    }

    public class H5TypeHandle : H5Handle
    {
        public H5TypeHandle(Handle handle) : base(handle, H5T.close) { }
    }

    public class H5FileHandle : H5LocationHandle
    {
        public H5FileHandle(Handle handle) : base(handle, H5F.close) { }
    }

    public class H5GroupHandle : H5LocationHandle
    {
        public H5GroupHandle(Handle handle) : base(handle, H5G.close) { }
    }

    public abstract class H5LocationHandle : H5Handle
    {
        protected H5LocationHandle(Handle handle, Func<Handle, int> closer) : base(handle, closer) { }
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
#if DEBUG
        public static Dictionary<Handle, string> Handles { get; private set; } = new();
#endif

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
        public const Handle InvalidHandleValue = -1;

        internal bool IsDisposed => Handle <= 0;

        protected H5Handle(Handle handle, Func<Handle, int> closer)
        {
            Debug.WriteLine($"Handle opened {handle}: {GetType().Name}");

            handle.ThrowIfNotValid();
            Handle = handle;
            CloseHandle = closer;

#if DEBUG
            Handles.Add(handle, Environment.StackTrace);
#endif
        }

        public static implicit operator Handle(H5Handle h)
        {
            return h.Handle;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                Debug.WriteLine($"Closing handle {Handle}");

                var err = CloseHandle(Handle);

#if DEBUG
                Handles.Remove(Handle);
#endif

                Handle = InvalidHandleValue;
                err.ThrowIfError();
            }
        }
    }

    public static class H5ThrowExtensions
    {
        public static void ThrowIfNotValid<THandle>(H5Object<THandle> h5Object) where THandle : H5Handle
        {
            // TODO: give more information, specific exception
            if (h5Object.Handle.Handle <= 0)
            {
                throw new InvalidOperationException("Bad handle");
            }
        }

        public static void ThrowIfNotValid(this H5Handle handle)
        {
            // TODO: give more information, specific exception
            if (handle.Handle <= 0)
            {
                throw new InvalidOperationException("Bad handle");
            }
        }

        public static void ThrowIfNotValid(this Handle handle)
        {
            // TODO: give more information, specific exception
            if (handle <= 0)
            {
                throw new InvalidOperationException("Bad handle");
            }
        }

        public static void ThrowIfError(this int err)
        {
            // TODO: give more information, specific exception
            if (err < 0)
            {
                throw new InvalidOperationException("Error");
            }
        }
    }
}
