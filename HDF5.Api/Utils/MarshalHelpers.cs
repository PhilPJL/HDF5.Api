#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif

namespace HDF5.Api.Utils
{
    internal static unsafe class MarshalHelpers
    {
#if NETSTANDARD
        public static string PtrToStringUTF8(IntPtr native)
        {
            int size = GetNativeDataSize(native);

            byte[] array = new byte[size];
            Marshal.Copy(native, array, 0, size);
            return Encoding.UTF8.GetString(array);

            static int GetNativeDataSize(IntPtr ptr)
            {
                int size;
                
                for (size = 0; Marshal.ReadByte(ptr, size) > 0; size++)
                {
                    // empty
                }

                return size;
            }
        }
#endif

        public static string GetName<T>(H5Object<T> h5Object, get_name_del getNameFunc) where T : H5Object<T>
        {
            // get length of buffer required
            int length = ((int)getNameFunc(h5Object, null, IntPtr.Zero)).ThrowIfError();

            if(length == 0)
            {
                return string.Empty;
            }

#if NET7_0_OR_GREATER
            if (length > 255)
            {
                using var bufferOwner = SpanOwner<byte>.Allocate(length + 1);
                return GetName(bufferOwner.Span);
            }
            else
            {
                Span<byte> buffer = stackalloc byte[length + 1];
                return GetName(buffer);
            }

            string GetName(Span<byte> buffer)
            {
                ((int)getNameFunc(h5Object, buffer, length + 1)).ThrowIfError();
                int nullTerminatorIndex = MemoryExtensions.IndexOf(buffer, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
                return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
            }
#else
            var buffer = new byte[length + 1];
            fixed (byte* bufferPtr = buffer)
            {
                ((int)getNameFunc(h5Object, bufferPtr, new IntPtr(length + 1))).ThrowIfError();

                Span<byte> bytes = buffer;
                // ReSharper disable once InvokeAsExtensionMethod
                var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
                return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
            }
#endif
        }

#if NET7_0_OR_GREATER
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate nint get_name_del(hid_t object_id, Span<byte> name, nint size);
#else
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ssize_t get_name_del(hid_t object_id, byte* name, size_t size);
#endif
    }
}
