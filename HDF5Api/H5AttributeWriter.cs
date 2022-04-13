using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5Api
{
    public static class H5AttributeWriter
    {
        internal static readonly ulong[] MaxDims = new ulong[] { H5S.UNLIMITED };

        public static IH5AttributeWriter<TInput> CreateAttributeWriter<TInput>
            (IH5Location location, Func<TInput, string> getAttributeName, IH5TypeAdapter<TInput> converter)
        {
            // NOTE: we're only interested in creating attributes currently, not reading them

            return new H5AttributeWriter<TInput>(location, converter, getAttributeName);
        }
    }

    public class H5AttributeWriter<TInput> : Disposable, IH5AttributeWriter<TInput>
    {
        public int RowsWritten { get; private set; }
        private H5Type Type { get; set; }
        private IH5TypeAdapter<TInput> Converter { get; }
        private Func<TInput, string> GetAttributeName { get; }
        private IH5Location Location { get; }

        public H5AttributeWriter(IH5Location location, IH5TypeAdapter<TInput> converter, Func<TInput, string> getAttributeName)
        {
            Location = location;
            Type = converter.GetH5Type();
            Converter = converter;
            GetAttributeName = getAttributeName;
        }

        public void Write(IEnumerable<TInput> recordsChunk)
        {
            // Single dimension (rank 1), unlimited length, chunk size.
            using var memorySpace = H5Space.CreateSimple(1, new ulong[] { 1 }, H5AttributeWriter.MaxDims);

            // Create an attribute-creation property list
            using var properyList = H5PropertyList.Create(H5P.ATTRIBUTE_CREATE);

            foreach (var record in recordsChunk)
            {
                // Create the attribute with our record type and chunk size.
                using var attribute = Location.CreateAttribute(GetAttributeName(record), Type, memorySpace, properyList);

                Converter.Write(WriteAdaptor(attribute, Type), Enumerable.Repeat(record, 1));
            }

            static Action<IntPtr> WriteAdaptor(H5Attribute attribute, H5Type type)
            {
                return (IntPtr buffer) => attribute.Write(type, buffer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Type?.Dispose();
                Type = null;
            }
        }
    }
}
