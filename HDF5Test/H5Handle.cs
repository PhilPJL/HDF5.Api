using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5Global
    {
        public static Version GetLibraryVersion()
        {
            uint major = 0;
            uint minor = 0;
            uint revision = 0;
            int err = H5.get_libversion(ref major, ref minor, ref revision);
            HD5Handle.AssertError(err);
            return new Version((int)major, (int)minor, (int)revision);
        }
    }

    public static class H5PropertyList
    {
        public static HD5PropertyListHandle Create(long classId)
        {
            Handle h = H5P.create(classId);
            HD5Handle.AssertHandle(h);
            return new HD5PropertyListHandle(h);
        }

        public static void SetChunk(HD5PropertyListHandle handle, int rank, ulong[] dims)
        {
            HD5Handle.AssertHandle(handle);
            int err = H5P.set_chunk(handle, rank, dims);
            HD5Handle.AssertError(err);
        }

        public static void EnableCompression(HD5PropertyListHandle handle, uint level)
        {
            HD5Handle.AssertHandle(handle);
            int err = H5P.set_deflate(handle, level);
            HD5Handle.AssertError(err);
        }
    }

    public static class H5File
    {
        public static HD5FileHandle Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            Handle h = H5F.create(name, flags);
            HD5Handle.AssertHandle(h);
            return new HD5FileHandle(h);
        }
    }

    public static class H5DataSet
    {
        public static HD5DataSetHandle Create(HD5LocationHandle location, string name,
            Handle rawRecordTypeId, HD5SpaceHandle spaceId, HD5PropertyListHandle propertyListId)
        {
            HD5Handle.AssertHandle(location);
            HD5Handle.AssertHandle(rawRecordTypeId);
            HD5Handle.AssertHandle(spaceId);
            HD5Handle.AssertHandle(propertyListId);
            Handle h = H5D.create(location, name, rawRecordTypeId, spaceId, H5P.DEFAULT, propertyListId);
            HD5Handle.AssertHandle(h);
            return new HD5DataSetHandle(h);
        }
    }

    public static class H5Group
    {
        public static HD5GroupHandle Create(HD5LocationHandle handle, string name)
        {
            HD5Handle.AssertHandle(handle);
            Handle h = H5G.create(handle, name);
            HD5Handle.AssertHandle(h);
            return new HD5GroupHandle(h);
        }
    }

    public static class H5Space
    {
        public static HD5SpaceHandle CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
        {
            Handle h = H5S.create_simple(rank, dims, maxdims);
            HD5Handle.AssertHandle(h);
            return new HD5SpaceHandle(h);
        }
    }

    public class HD5PropertyListHandle : HD5Handle
    {
        public HD5PropertyListHandle(Handle handle) : base(handle)
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

    public class HD5FileHandle : HD5LocationHandle
    {
        public HD5FileHandle(Handle handle) : base(handle)
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

    public class HD5GroupHandle : HD5LocationHandle
    {
        public HD5GroupHandle(Handle handle) : base(handle)
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

    public abstract class HD5LocationHandle : HD5Handle
    {
        protected HD5LocationHandle(Handle handle) : base(handle)
        {
        }
    }

    public class HD5DataSetHandle : HD5Handle
    {
        public HD5DataSetHandle(Handle handle) : base(handle)
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

    public class HD5SpaceHandle : HD5Handle
    {
        public HD5SpaceHandle(Handle handle) : base(handle)
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

    public abstract class HD5Handle : Disposable
    {
        public Handle Handle { get; protected set; }
        public const Handle InvalidHandle = -1;

        protected HD5Handle(Handle handle)
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

        public static implicit operator Handle(HD5Handle h)
        {
            AssertHandle(h.Handle);
            return h.Handle;
        }
    }
}
