namespace HDF5.Api.Tests
{
    [TestClass]
    public class H5ObjectTests : H5Test
    {
        [TestMethod]
        public void HasHandleTypeTest()
        {
            HandleCheck(() =>
            {
                using var file = CreateFile();

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
                var file = CreateFile();

                Assert.IsFalse(file.IsDisposed);
                file.Dispose();
                Assert.IsTrue(file.IsDisposed);
                file.Dispose();
            });
        }
    }
}