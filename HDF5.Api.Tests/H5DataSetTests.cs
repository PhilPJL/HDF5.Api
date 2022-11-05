using HDF5.Api.H5Types;

namespace HDF5.Api.Tests;

[TestClass]
public class H5DataSetTests : H5Test<H5DataSetTests>
{
    internal static H5DataSet CreateTestDataset<T>(H5Location<T> location, string dataSetName) where T : H5Location<T>
    {
        const int chunkSize = 1;

        using var memorySpace = H5Space.Create(new Dimension(chunkSize));
        using var propertyList = H5DataSet.CreateCreationPropertyList();

        // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
        propertyList.SetChunk(chunkSize);

        using var type = H5Type.GetEquivalentNativeType<long>();

        return location.CreateDataSet(dataSetName, type, memorySpace, propertyList);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateOnFile(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            Assert.IsTrue(file.DataSetExists(dataSetName));
            Assert.IsTrue(file.DataSetNames.Contains(dataSetName));
            Assert.AreEqual("/" + dataSetName, ds.Name);
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateDeleteOnFile(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            file.DeleteDataSet(dataSetName);

            Assert.IsFalse(file.DataSetExists(dataSetName));
            Assert.IsFalse(file.DataSetNames.Contains(dataSetName));
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void GetCreationPropertyListSucceeds(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            Assert.IsTrue(file.DataSetExists(dataSetName));

            using var dscpl = ds.GetCreationPropertyList();

            // Set same params as for CreateTestDataset
            using var cpl1 = H5DataSet.CreateCreationPropertyList();
            cpl1.SetChunk(1);

            Assert.IsTrue(cpl1.Equals(dscpl));
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateOnGroup(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            using var group = file.CreateGroup("aGroup");

            // Create test ds on file/group
            using var ds = CreateTestDataset(group, dataSetName);

            Assert.IsTrue(group.DataSetExists(dataSetName));
            Assert.IsFalse(group.DataSetExists(dataSetName + "x"));

            // Create test ds on file/group/sub-group
            using var group2 = group.CreateGroup("bGroup");
            using var ds2 = CreateTestDataset(group2, dataSetName);

            Assert.IsTrue(group2.DataSetExists(dataSetName));
            Assert.IsFalse(group2.DataSetExists(dataSetName + "y"));

            // Test using rooted path
            Assert.IsTrue(file.GroupPathExists("/aGroup/bGroup"));
            Assert.IsFalse(file.GroupPathExists("/aGroup/bGroupx"));

            Assert.IsTrue(file.DataSetExists($"/aGroup/bGroup/{dataSetName}"));
            Assert.IsFalse(file.DataSetExists($"/aGroup/bGroup/{dataSetName}x"));

            // 1 file, 2 groups, 2 data-sets
            Assert.AreEqual(5, file.GetObjectCount());
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateDeleteOnGroup(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            using var group = file.CreateGroup("aGroup");

            // Create test ds on file/group
            using var ds = CreateTestDataset(group, dataSetName);

            group.DeleteDataSet(dataSetName);

            Assert.IsFalse(group.DataSetExists(dataSetName));
            Assert.IsFalse(group.DataSetExists(dataSetName + "x"));
        },
        fileNameSuffix);
    }

    #region Attributes

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateWriteReadDeleteAttributesSucceeds(string fileNameSuffix, string dataSetName)
    {
        HandleCheck2((file) =>
        {
            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(ds);
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateIterateAttributesSucceeds(string fileNameSuffix, string dataSetName)
    {
        HandleCheck((file) =>
        {
            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            H5AttributeTests.CreateIterateAttributesSucceeds(ds);
        },
        fileNameSuffix);
    }

    #endregion
}
