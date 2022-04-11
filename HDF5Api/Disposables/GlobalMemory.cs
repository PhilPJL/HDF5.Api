using System;
using System.Runtime.InteropServices;

namespace HDF5Api.Disposables
{
    public class GlobalMemory : Disposable
    {
        public GlobalMemory(int size)
        {
            IntPtr = Marshal.AllocHGlobal(size);
        }

        public IntPtr IntPtr { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && IntPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(IntPtr);
                IntPtr = IntPtr.Zero;
            }
        }

        public static unsafe implicit operator void*(GlobalMemory memory)
        {
            return memory.IntPtr.ToPointer();
        }

        public static implicit operator IntPtr(GlobalMemory memory)
        {
            return memory.IntPtr;
        }
    }
}
