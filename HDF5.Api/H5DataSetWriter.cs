namespace HDF5.Api;

/// <summary>
///     Factory class for creating data-set writers.
/// </summary>
public static class H5DataSetWriter
{
    public static IH5DataSetWriter<TInput> CreateOneDimensionalDataSetWriter<TInput>
        (IH5Location location, string dataSetName, IH5TypeAdapter<TInput> converter, int chunkSize, int compressionLevel = 0)
    {
        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5Space.Create(chunkSize);

        // Create a dataset-creation property list
        using var propertyList = H5DataSet.CreatePropertyList(PropertyListType.Create);

        // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
        propertyList.SetChunk(chunkSize);

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
