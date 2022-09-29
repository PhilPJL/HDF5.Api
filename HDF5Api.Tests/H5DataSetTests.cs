using HDF.PInvoke;

namespace HDF5Api.Tests;

[TestClass]
public class H5DataSetTests : H5Test
{
    private const string Path = "test.h5";

    internal static H5DataSet CreateTestDataset(IH5Location location, string dataSetName)
    {
        const int chunkSize = 1;

        using var memorySpace = H5Space.Create(new Dimension(chunkSize));

        using var propertyList = H5PropertyListNativeMethods.Create(H5P.DATASET_CREATE);

        // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
        propertyList.SetChunk(1, chunkSize);

        using var type = H5Type.GetNativeType<long>();

        return location.CreateDataSet(dataSetName, type, memorySpace, propertyList);
    }

    [TestMethod]
    public void CreateOnFile()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create test ds
            using var ds = CreateTestDataset(file, "aDataSet");

            Assert.IsTrue(file.DataSetExists("aDataSet"));
        });
    }

    [TestMethod]
    public void CreateOnGroup()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            using var group = file.CreateGroup("aGroup");

            // Create test ds on file/group
            using var ds = CreateTestDataset(group, "aDataSet");

            Assert.IsTrue(group.DataSetExists("aDataSet"));
            Assert.IsFalse(group.DataSetExists("aDataSet1"));

            // Create test ds on file/group/sub-group
            using var group2 = group.CreateGroup("bGroup");
            using var ds2 = CreateTestDataset(group2, "bDataSet");

            Assert.IsTrue(group2.DataSetExists("bDataSet"));
            Assert.IsFalse(group2.DataSetExists("bDataSet1"));

            // Test using rooted path
            Assert.IsTrue(file.GroupPathExists("/aGroup/bGroup"));
            Assert.IsFalse(file.GroupPathExists("/aGroup/bGroup1"));

            Assert.IsTrue(file.DataSetExists("/aGroup/bGroup/bDataSet"));
            Assert.IsFalse(file.DataSetExists("/aGroup/bGroup/bDataSet1"));

            // 1 file, 2 groups, 2 data-sets
            Assert.AreEqual(5, file.GetObjectCount());
        });
    }

    #region Attributes

    [TestMethod]
    public void CreateWriteReadDeleteAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create test ds
            using var ds = CreateTestDataset(file, "aDataSet");

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(ds);
        });
    }

    [TestMethod]
    public void CreateIterateAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create test ds
            using var ds = CreateTestDataset(file, "aDataSet");

            H5AttributeTests.CreateIterateAttributesSucceeds(ds);
        });
    }
    #endregion
}
