namespace HDF5Api
{
    /// <summary>
    /// Factory class for creating data-set writers.
    /// </summary>
    public static class H5DataSetWriter
    {
        internal static readonly ulong[] MaxDims1D = new ulong[] { H5S.UNLIMITED };

        public static IH5DataSetWriter<TInput> CreateOneDimensionalDataSetWriter<TInput>
            (IH5Location location, string dataSetName, IH5TypeAdapter<TInput> converter, uint compressionLevel = 1, int chunkSize = 100)
        {
            // NOTE: we're only interested in creating a data set currently, not opening an existing one

            // Single dimension (rank 1), unlimited length, chunk size.
            using var memorySpace = H5Space.CreateSimple(1, new ulong[] { (ulong)chunkSize }, MaxDims1D);

            // Create a dataset-creation property list
            using var properyList = H5PropertyList.Create(H5P.DATASET_CREATE);

            // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
            properyList.SetChunk(1, new ulong[] { (ulong)chunkSize });

            if (compressionLevel > 0)
            {
                properyList.EnableDeflateCompression(compressionLevel);
            }

            var h5CompoundType = converter.GetH5Type();

            // Create a dataset with our record type and chunk size.
            var dataSet = location.CreateDataSet(dataSetName, h5CompoundType, memorySpace, properyList);

            // Writer owns and disposes/releases the data-set.
            return new H5DataSetWriter1D<TInput>(dataSet, h5CompoundType, converter, true);
        }
    }
}
