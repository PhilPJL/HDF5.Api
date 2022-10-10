namespace HDF5.Api.Tests;

[TestClass]
public class AttributeWriterTests : H5Test
{
    private const string Path = "test2.h5";

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

            using var writer = H5AttributeWriter
                .CreateAttributeWriter(file, (r) => $"AttName: {r.Id}", TestRecordAdapter.Default);

            Enumerable.Range(0, 500)
                .Select(i => new TestRecord { Id = i })
                .Buffer(50)
                .ForEach(b => writer.Write(b));
        });
    }
}