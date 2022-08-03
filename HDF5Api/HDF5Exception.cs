using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace HDF5Api
{
    [Serializable]
    public sealed class Hdf5Exception : Exception
    {
        public Hdf5Exception()
        {
        }

        public Hdf5Exception(string message) : base(message)
        {
        }

        public Hdf5Exception(string message, Exception inner) : base(message, inner)
        {
        }

        private Hdf5Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static Hdf5Exception Create(string function, int err)
        {
            return new Hdf5Exception(string.Format(CultureInfo.InvariantCulture, "Error calling: {0}. err:{1}", function, err));
        }

        public static Hdf5Exception Create(string function, long err)
        {
            return new Hdf5Exception(string.Format(CultureInfo.InvariantCulture, "Error calling: {0}. err:{1}", function, err));
        }
    }
}
