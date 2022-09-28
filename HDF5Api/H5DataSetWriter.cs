namespace HDF5Api;

/// <summary>
///     Factory class for creating data-set writers.
/// </summary>
public static class H5DataSetWriter
{
    public static IH5DataSetWriter<TInput> CreateOneDimensionalDataSetWriter<TInput>
        (IH5Location location, string dataSetName, IH5TypeAdapter<TInput> converter, int chunkSize, uint compressionLevel)
    {
        // NOTE: we're only interested in creating a data set currently, not opening an existing one

        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SpaceNativeMethods.CreateSimple(new Dimension((ulong)chunkSize));

        // Create a dataset-creation property list
        using var propertyList = H5PropertyListNativeMethods.Create(H5P.DATASET_CREATE);

        // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
        propertyList.SetChunk(1, (ulong)chunkSize);

        if (compressionLevel > 0)
        {
            propertyList.EnableDeflateCompression(compressionLevel);
        }

        var h5CompoundType = converter.GetH5Type();

        // Create a dataset with our record type and chunk size.
        var dataSet = location.CreateDataSet(dataSetName, h5CompoundType, memorySpace, propertyList);

        // Writer owns and disposes/releases the data-set.
        return new H5DataSetWriter1D<TInput>(dataSet, h5CompoundType, converter, true);
    }
}
