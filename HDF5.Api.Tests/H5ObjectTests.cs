namespace HDF5.Api.Tests
{
    [TestClass]
    public class H5ObjectTests : H5Test
    {
        private const string Path = "test.h5";

        [TestMethod]
        public void HasHandleTypeTest()
        {
            HandleCheck(() =>
            {
                // Ensure no existing file
                File.Delete(Path);
                Assert.IsFalse(File.Exists(Path));

                // Create new file
                using var file = H5File.Create(Path);
                Assert.IsTrue(File.Exists(Path));

                file.AssertHasHandleType(HandleType.File);
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.Group));
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.Attribute));
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.DataSet));
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.PropertyList));
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.Type));
                Assert.ThrowsException<Hdf5Exception>(() => file.AssertHasHandleType(HandleType.Space));

                // Create group
                using var group = file.CreateGroup("group");
                group.AssertHasHandleType(HandleType.Group);

                // Create space
                using var space = H5Space.Create(new Dimension(1, 2));
                space.AssertHasHandleType(HandleType.Space);

                // Create type
                using var type = H5Type.GetNativeType<int>();
                type.AssertHasHandleType(HandleType.Type);

                // Create property list
                using var plist = H5Attribute.CreatePropertyList(PropertyListType.Create);
                plist.AssertHasHandleType(HandleType.PropertyList);

                // Create attribute
                using var att = group.CreateAttribute("att", type, space, plist);
                att.AssertHasHandleType(HandleType.Attribute);

                // Create data-set
                // TODO:
            });
        }

        [TestMethod]
        public void DisposeAlreadyDisposedObjectDoesntThrowTest()
        {
            HandleCheck(() =>
            {
                // Ensure no existing file
                File.Delete(Path);
                Assert.IsFalse(File.Exists(Path));

                // Create new file
                var file = H5File.Create(Path);
                Assert.IsFalse(file.IsDisposed);
                file.Dispose();
                Assert.IsTrue(file.IsDisposed);
                file.Dispose();
            });
        }
    }
}