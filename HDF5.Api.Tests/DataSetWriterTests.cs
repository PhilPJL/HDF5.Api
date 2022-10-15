using System.Diagnostics;

namespace HDF5.Api.Tests;

[TestClass]
public class DataSetWriterTests : H5Test
{
    [TestMethod]
    public void DataSetWriterTestCompressed()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            using var writer = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "TestRecords", TestRecordAdapter.Default, 10, 5);

            Enumerable.Range(0, 50000)
                .Select(i => new TestRecord { Id = i })
                .Buffer(50)
                .ForEach(b => writer.Write(b));

            Debug.WriteLine($"Compressed={file.Size}");
        });
    }

    [TestMethod]
    public void DataSetWriterTestUncompressed()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            using var writer = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "TestRecords", TestRecordAdapter.Default, 10, 0);

            Enumerable.Range(0, 50000)
                .Select(i => new TestRecord { Id = i })
                .Buffer(50)
                .ForEach(b => writer.Write(b));

            Debug.WriteLine($"Uncompressed={file.Size}");
        });
    }
}
