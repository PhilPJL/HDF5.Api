namespace HDF5Api.Tests;

[TestClass]
public class H5PropertyListTests : H5Test
{
    private const string Path = "test.h5";

    [TestMethod]
    public void FileCreateDefaultPropertyList()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file with default property lists
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Is create list the same as default?
            using var fpc1 = file.GetPropertyList(PropertyListType.Create);
            using var fpc2 = H5File.CreatePropertyList(PropertyListType.Create);
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));

            //using var fpa1 = file.GetPropertyList(PropertyListType.Access);
            //using var fpa2 = H5File.CreatePropertyList(PropertyListType.Access);
            //Assert.IsTrue(fpa1.IsEqualTo(fpa2));
        });
    }

/*    [TestMethod]
    public void FileCreateNonDefaultPropertyList()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            using var pl = H5File.CreatePropertyList(PropertyListType.Create);
            pl.ReadAttribute
            // Create new file with default property lists
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Is create list the same as default?
            using var fpc1 = file.GetPropertyList(PropertyListType.Create);
            using var fpc2 = H5File.CreatePropertyList(PropertyListType.Create);
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));

            //using var fpa1 = file.GetPropertyList(PropertyListType.Access);
            //using var fpa2 = H5File.CreatePropertyList(PropertyListType.Access);
            //Assert.IsTrue(fpa1.IsEqualTo(fpa2));
        });
    }
*/
    [TestMethod]
    public void PropertyListHelpersTest()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // file property lists
            using var fpc1 = file.GetPropertyList(PropertyListType.Create);
            using var fpc2 = H5File.CreatePropertyList(PropertyListType.Create);
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));

            //using var fpa1 = file.GetPropertyList(PropertyListType.Access);
            //using var fpa2 = H5File.CreatePropertyList(PropertyListType.Access);
            //Assert.IsTrue(fpa1.IsEqualTo(fpa2));

            // group 
            using var group = file.CreateGroup("group");
            using var gpc1 = group.GetPropertyList(PropertyListType.Create);
            using var gpc2 = H5Group.CreatePropertyList(PropertyListType.Create);
            Assert.IsTrue(gpc1.IsEqualTo(gpc2));

            // dataset
            //using var ds = group.C

            // attribute
            file.CreateAndWriteAttribute("IntAttribute", 1);
            using var att = file.OpenAttribute("IntAttribute");
            using var apc1 = att.GetPropertyList(PropertyListType.Create);
            using var apc2 = H5Attribute.CreatePropertyList(PropertyListType.Create);
            Assert.IsTrue(apc1.IsEqualTo(apc2));

            using var apc12 = H5Attribute.CreatePropertyList(PropertyListType.Create);
            file.CreateAndWriteAttribute("Int2Attribute", 1, apc12);
            using var att1 = file.OpenAttribute("IntAttribute");
            using var apc11 = att1.GetPropertyList(PropertyListType.Create);

            Assert.IsTrue(apc11.IsEqualTo(apc12));
        });
    }
}