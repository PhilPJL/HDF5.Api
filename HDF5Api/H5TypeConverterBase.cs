using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Api
{
    public abstract class H5TypeConverterBase
    {
        protected static readonly ASCIIEncoding Ascii = new();

        protected unsafe void CopyString(string source, byte* destination, int destinationSizeInBytes)
        {
            byte[] sourceBytes = Ascii.GetBytes(source);

            var msg = $"Profile: The provided string '{source}' has length {source.Length} which exceeds the maximum destination length of {destinationSizeInBytes}.";

            if (source.Length > destinationSizeInBytes)
            {
                throw new InvalidOperationException(msg);
            }

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(sourceBytes, 0).ToPointer(), destination, destinationSizeInBytes, source.Length);
        }
    }
}
