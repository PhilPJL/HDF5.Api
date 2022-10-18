using System.ComponentModel;

namespace HDF5.Api.Tests;

[TestClass]
public class H5PropertyListTests : H5Test<H5PropertyListTests>
{
    [TestMethod]
    public void FileCreateDefaultPropertyList()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            // Is create list the same as default?
            using var fpc1 = file.GetCreationPropertyList();
            using var fpc2 = H5File.CreateCreationPropertyList();
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));

            // TODO: why doesn't this work?
            //using var fpa1 = file.GetPropertyList(PropertyListType.Access);
            //using var fpa2 = H5File.CreatePropertyList(PropertyListType.Access);
            //Assert.IsTrue(fpa1.IsEqualTo(fpa2));
        });
    }

    // TODO: doesn't work - ask HDF Group
    /*    [TestMethod]
        public void FileAccessDefaultPropertyList()
        {
            HandleCheck(() =>
            {
                // Ensure no existing file
                File.Delete(Path);
                Assert.IsFalse(File.Exists(Path));

                using var fpa1 = H5File.CreatePropertyList(PropertyListType.Access);

                // Create new file with default property lists
                using var file = H5File.Create(Path, true, null, fpa1);
                Assert.IsTrue(File.Exists(Path));

                using var fpa2 = file.GetPropertyList(PropertyListType.Access);
                Assert.IsTrue(fpa1.IsEqualTo(fpa2));
            });
        }
    */


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
            using var file = CreateFile();

            // file property lists
            using var fpc1 = file.GetCreationPropertyList();
            using var fpc2 = H5File.CreateCreationPropertyList();
            Assert.IsTrue(fpc1.IsEqualTo(fpc2));

            // TODO: why doesn't this work?
            //using var fpa1 = file.GetPropertyList(PropertyListType.Access);
            //using var fpa2 = H5File.CreatePropertyList(PropertyListType.Access);
            //Assert.IsTrue(fpa1.IsEqualTo(fpa2));

            // group 
            using var group = file.CreateGroup("group");
            using var gpc1 = group.GetCreationPropertyList();
            using var gpc2 = H5Group.CreateCreationPropertyList();
            Assert.IsTrue(gpc1.IsEqualTo(gpc2));

            // dataset
            //using var ds = group.C

            // attribute
            file.CreateAndWriteAttribute("IntAttribute", 1);
            using var att = file.OpenAttribute("IntAttribute");
            using var apc1 = att.GetCreationPropertyList();
            using var apc2 = H5Attribute.CreateCreationPropertyList();
            Assert.IsTrue(apc1.IsEqualTo(apc2));
        });
    }

    // TODO: data set property lists
}