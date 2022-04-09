using System;

namespace HDF5Api
{
    public static class H5DataSetWriter
    {
        public static readonly ulong[] MaxDims = new ulong[] { H5S.UNLIMITED };

        public static IH5DataSetWriter<TInput, TOutput> CreateOneDimensionalDataSetWriter<TInput, TOutput>(IH5Location location, string dataSetName, IH5TypeConverter<TInput, TOutput> converter, int chunkSize = 100) where TOutput : struct
        {
            // NOTE: we're only interested in creating a data set currently, not opening an existing one

            // Single dimension (rank 1), unlimited length, chunk size.
            using var memorySpace = H5Space.CreateSimple(1, new ulong[] { (ulong)chunkSize }, MaxDims);
            Console.WriteLine($"Created space: {memorySpace}");

            // Create a dataset-creation property list
            using var properyList = H5PropertyList.Create(H5P.DATASET_CREATE);

            // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
            properyList.SetChunk(1, new ulong[] { (ulong)chunkSize });

            // TODO: investigate performance of compression and different compression types
            //properyList.EnableDeflateCompression(6);

            var h5CompoundType = converter.CreateH5Type();

            // Create a dataset with our record type and chunk size.
            // TODO: get h5CompoundType from CompoundType and own h5CompoundType - get rid of typeFactory?
            var dataSet = location.CreateDataSet(dataSetName, h5CompoundType, memorySpace, properyList);

            // Writer owns and disposes/releases the data-set.
            return new H5DataSetWriter1D<TInput, TOutput>(dataSet, h5CompoundType, converter.Convert, true);
        }
    }
}
