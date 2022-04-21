using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace HDF5Api
{
    [Serializable]
    public sealed class HDF5Exception : Exception
    {
        public HDF5Exception()
        {
        }

        public HDF5Exception(string message) : base(message)
        {
        }

        public HDF5Exception(string message, Exception inner) : base(message, inner)
        {
        }

        private HDF5Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static HDF5Exception Create(string function, int err)
        {
            return new HDF5Exception(string.Format(CultureInfo.InvariantCulture, "Error calling: {0}. err:{1}", function, err));
        }

        public static HDF5Exception Create(string function, long err)
        {
            return new HDF5Exception(string.Format(CultureInfo.InvariantCulture, "Error calling: {0}. err:{1}", function, err));
        }
    }
}
