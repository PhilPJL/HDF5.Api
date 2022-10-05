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

            using var intervalRecordWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "IntervalRecords", TestRecordAdapter.Default, 10, 5);

            for (int i = 0; i < 1000; i++)
            {
                intervalRecordWriter.Write(Enumerable.Repeat(new TestRecord(), 50).ToArray());
            }

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

            using var intervalRecordWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(file, "IntervalRecords", TestRecordAdapter.Default, 10, 0);

            for (int i = 0; i < 1000; i++)
            {
                intervalRecordWriter.Write(Enumerable.Repeat(new TestRecord(), 50).ToArray());
            }

            Debug.WriteLine($"Uncompressed={file.GetSize()}");
        });
    }

}