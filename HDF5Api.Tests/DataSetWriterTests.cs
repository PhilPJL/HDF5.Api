using System.Diagnostics;

namespace HDF5Api.Tests;

[TestClass]
public class DataSetWriterTests : H5Test
{
    private const string Path = "test.h5";
    private const string Path1 = "test1.h5";

    [TestMethod]
    public void DataSetWriterTestCompressed()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            using var writer = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "TestRecords", TestRecordAdapter.Default, 10, 5);

            Enumerable.Range(0, 50000)
                .Select(i => new TestRecord { Id = i })
                .Buffer(50)
                .ForEach(b => writer.Write(b));

            Debug.WriteLine($"Compressed={file.GetSize()}");
        });
    }

    [TestMethod]
    public void DataSetWriterTestUncompressed()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path1);
            Assert.IsFalse(File.Exists(Path1));

            // Create new file
            using var file = H5File.Create(Path1);
            Assert.IsTrue(File.Exists(Path1));

            using var writer = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "TestRecords", TestRecordAdapter.Default, 10, 0);

            Enumerable.Range(0, 50000)
                .Select(i => new TestRecord { Id = i })
                .Buffer(50)
                .ForEach(b => writer.Write(b));

            Debug.WriteLine($"Uncompressed={file.GetSize()}");
        });
    }

}
