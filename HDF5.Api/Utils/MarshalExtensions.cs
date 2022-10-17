namespace HDF5.Api.Utils
{
#if NETSTANDARD
    internal unsafe class MarshalExtensions
    {
        public static string PtrToStringUTF8(IntPtr native)
        {
            int size = GetNativeDataSize(native);

            // TODO: use stackalloc?
            byte[] array = new byte[size];
            Marshal.Copy(native, array, 0, size);
            return Encoding.UTF8.GetString(array);

            static int GetNativeDataSize(IntPtr ptr)
            {
                // TODO: is there a better way to determine the length?
                int size;
                for (size = 0; Marshal.ReadByte(ptr, size) > 0; size++) ;
                return size;
            }
        }
    }
#endif
}
