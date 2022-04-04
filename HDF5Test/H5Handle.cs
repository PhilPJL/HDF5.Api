using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5PropertyListHandle : H5Handle
    {
        public H5PropertyListHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5P.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public class H5TypeHandle : H5Handle
    {
        public H5TypeHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5T.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public class H5FileHandle : H5LocationHandle
    {
        public H5FileHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5F.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public class H5GroupHandle : H5LocationHandle
    {
        public H5GroupHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5G.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public abstract class H5LocationHandle : H5Handle
    {
        protected H5LocationHandle(Handle handle) : base(handle)
        {
        }
    }

    public class H5DataSetHandle : H5Handle
    {
        public H5DataSetHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5D.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public class H5SpaceHandle : H5Handle
    {
        public H5SpaceHandle(Handle handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Handle > 0)
            {
                int err = H5S.close(Handle);
                ClearHandle();
                AssertError(err);
            }
        }
    }

    public abstract class H5Handle : Disposable
    {
        public Handle Handle { get; protected set; }
        public const Handle InvalidHandle = -1;

        protected H5Handle(Handle handle)
        {
            Handle = handle;
        }

        public static void AssertHandle(Handle handle)
        {
            if (handle <= 0)
            {
                throw new InvalidOperationException("Bad handle");
            }
        }

        public static void AssertError(int err)
        {
            if (err < 0)
            {
                throw new InvalidOperationException("Error");
            }
        }

        protected void ClearHandle()
        {
            Handle = InvalidHandle;
        }

        public static implicit operator Handle(H5Handle h)
        {
            AssertHandle(h.Handle);
            return h.Handle;
        }
    }
}
