namespace HDF5.Api.Tests;

[TestClass]
public class H5PropertyListTests : H5Test<H5PropertyListTests>
{
    [TestMethod]
    public void FileCreationPropertyList()
    {
        HandleCheck((file) =>
        {
            // Is create list the same as default?
            using var fpc1 = new H5FileCreationPropertyList();
            using var fpc2 = H5File.CreateCreationPropertyList();
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));
            Assert.IsTrue(fpc1.Equals(fpc2));

            Assert.AreEqual(fpc1.GetClass(), fpc2.GetClass());
            Assert.AreNotEqual(fpc1.GetHashCode(), fpc2.GetHashCode());
        });
    }

    [TestMethod]
    public void FileAccessPropertyList()
    {
        HandleCheck((file) =>
        {
            // Is create list the same as default?
            using var fpc1 = new H5FileAccessPropertyList();
            using var fpc2 = H5File.CreateAccessPropertyList();
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));
            Assert.IsTrue(fpc1.Equals(fpc2));
        });
    }

    [TestMethod]
    public void PropertyListHelpersTest()
    {
        HandleCheck((file) =>
        {
            // file property lists
            using var fpc1 = file.GetCreationPropertyList();
            using var fpc2 = H5File.CreateCreationPropertyList();
            using var fpc3 = new H5FileCreationPropertyList();
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));
            Assert.IsTrue(fpc1.IsEqualTo(fpc3));

            // To exercise null checks
            Assert.IsFalse(fpc1.IsEqualTo(null));
            Assert.IsFalse(((object)fpc1).Equals(null));

            // This isn't true - HDF5 quirk
            //using var fac1 = file.GetAccessPropertyList();
            using var fac2 = H5File.CreateAccessPropertyList();
            using var fac3 = new H5FileAccessPropertyList();
            //Assert.IsTrue(fac1.IsEqualTo(fac2));
            Assert.IsTrue(fac2.IsEqualTo(fac3));

            // group 
            using var group = file.CreateGroup("group");
            using var gpc1 = group.GetCreationPropertyList();
            using var gpc2 = H5Group.CreateCreationPropertyList();
            using var gpc3 = new H5GroupCreationPropertyList();
            Assert.IsTrue(gpc1.IsEqualTo(gpc2));
            Assert.IsTrue(gpc1.Equals(gpc3));

            // dataset
            using var ds = CreateTestDataset(file, "dataSet");
            using var dspc1 = ds.GetCreationPropertyList();
            using var dspc2 = H5DataSet.CreateCreationPropertyList();
            using var dspc3 = new H5DataSetCreationPropertyList();
            dspc2.SetChunk(1); // to match CreateTestDataset
            dspc3.SetChunk(1); // to match CreateTestDataset
            Assert.IsTrue(dspc1.Equals(dspc2));
            Assert.IsTrue(dspc1.IsEqualTo(dspc3));

            // This isn't true - HDF5 quirk
            //using var dspa1 = ds.GetAccessPropertyList();
            using var dspa2 = H5DataSet.CreateAccessPropertyList();
            using var dspa3 = new H5DataSetAccessPropertyList();
            //Assert.IsTrue(dspa1.IsEqualTo(dspa2));
            Assert.IsTrue(dspa2.IsEqualTo(dspa3));

            // attribute
            file.CreateAndWriteAttribute("IntAttribute", 1);
            using var att = file.OpenAttribute("IntAttribute");
            using var apc1 = att.GetCreationPropertyList();
            using var apc2 = H5Attribute.CreateCreationPropertyList();
            using var apc3 = new H5AttributeCreationPropertyList();
            Assert.IsTrue(apc1.IsEqualTo(apc2));

            // data type
            using var dtc1 = H5Type.CreateCreationPropertyList();
            using var dtc2 = new H5DataTypeCreationPropertyList();
            Assert.IsTrue(dtc1.Equals(dtc2));
            using var dta1 = H5Type.CreateAccessPropertyList();
            using var dta2 = new H5DataTypeAccessPropertyList();
            Assert.IsTrue(dta1.Equals(dta2));

            // link creation
            using var lc1 = H5Link.CreateCreationPropertyList(CharacterSet.Ascii, false);
            using var lc2 = new H5LinkCreationPropertyList();

            Assert.IsTrue(((object)lc1).Equals(lc2));
        });

        static H5DataSet CreateTestDataset<T>(H5Location<T> location, string dataSetName) where T : H5Location<T>
        {
            const int chunkSize = 1;

            using var memorySpace = H5Space.Create(new Dimension(chunkSize));
            using var propertyList = H5DataSet.CreateCreationPropertyList();

            propertyList.SetChunk(1);

            using var type = H5Type.GetEquivalentNativeType<long>();

            return location.CreateDataSet(dataSetName, type, memorySpace, propertyList);
        }
    }
}