namespace HDF5.Api.Tests;

[TestClass]
public class H5TypeTests : H5Test
{
    [TestMethod]
    public void CreateCommittedDataType()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile("CreateCommittedDataType.h5");

            using var bigStringType = H5Type.CreateFixedLengthStringType(1000);
            file.Commit("bigstring", bigStringType);

            Assert.IsTrue(file.NamedDataTypeNames.Contains("bigstring"));

            using var group = file.CreateGroup("group");
            using var smallStringType = H5Type.CreateFixedLengthStringType(10);
            group.Commit("smallstring", smallStringType);

            Assert.IsTrue(group.NamedDataTypeNames.Contains("smallstring"));
        });
    }
}