namespace HDF5.Api.Tests;

[TestClass]
public class H5DataSetTests : H5Test<H5DataSetTests>
{
    internal static H5DataSet CreateTestDataset(IH5Location location, string dataSetName)
    {
        const int chunkSize = 1;

        using var memorySpace = H5Space.Create(new Dimension(chunkSize));
        using var propertyList = H5DataSet.CreateCreationPropertyList();

        // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
        propertyList.SetChunk(chunkSize);

        using var type = H5Type.GetNativeType<long>();

        return location.CreateDataSet(dataSetName, type, memorySpace, propertyList);
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateOnFile(string fileNameSuffix, string dataSetName)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            Assert.IsTrue(file.DataSetExists(dataSetName));
        });
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateOnGroup(string fileNameSuffix, string dataSetName)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

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
            Assert.IsTrue(file.GroupPathExists($"/aGroup/bGroup"));
            Assert.IsFalse(file.GroupPathExists($"/aGroup/bGroupx"));

            Assert.IsTrue(file.DataSetExists($"/aGroup/bGroup/{dataSetName}"));
            Assert.IsFalse(file.DataSetExists($"/aGroup/bGroup/{dataSetName}x"));

            // 1 file, 2 groups, 2 data-sets
            Assert.AreEqual(5, file.GetObjectCount());
        });
    }

    #region Attributes

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateWriteReadDeleteAttributesSucceeds(string fileNameSuffix, string dataSetName)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(ds);
        });
    }

    [TestMethod]
    [DataRow("ascii", "data-set-name-ascii")]
    [DataRow("utf8", "data-set-name-utf8-ᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠ")]
    public void CreateIterateAttributesSucceeds(string fileNameSuffix, string dataSetName)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

            // Create test ds
            using var ds = CreateTestDataset(file, dataSetName);

            H5AttributeTests.CreateIterateAttributesSucceeds(ds);
        });
    }

    #endregion
}
