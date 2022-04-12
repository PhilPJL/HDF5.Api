using System;
using System.Linq;

namespace HDF5Api
{
    public static class H5AttributeWriter
    {
        private static readonly ulong[] MaxDims = new ulong[] { H5S.UNLIMITED };

        // TODO: use same approach as DataSetWriter
        public static H5Attribute CreateAndWriteAttribute<TInput>(IH5Location location, string attributeName, TInput input, IH5TypeAdapter<TInput> converter)
        {
            // Single dimension (rank 1), unlimited length, chunk size.
            using var memorySpace = H5Space.CreateSimple(1, new ulong[] { 1 }, MaxDims);

            // Create a attribute-creation property list
            using var properyList = H5PropertyList.Create(H5P.ATTRIBUTE_CREATE);

            using var h5CompoundType = converter.GetH5Type();

            // Create a dataset with our record type and chunk size.
            var attribute = location.CreateAttribute(attributeName, h5CompoundType, memorySpace, properyList);

            converter.Write(WriteAdaptor(attribute, h5CompoundType), Enumerable.Repeat(input, 1));

            return attribute;

            static Action<IntPtr> WriteAdaptor(H5Attribute attribute, H5Type type)
            {
                return (IntPtr buffer) => attribute.Write(type, buffer);
            }
        }
    }
}
