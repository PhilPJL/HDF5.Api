using System;

namespace HDF5Test
{
    public static class ConversionExtensions
    {
        public static double[] ToDouble(this byte[] bytes)
        {
            double[] result = new double[bytes.Length / 8];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }
    }
}

