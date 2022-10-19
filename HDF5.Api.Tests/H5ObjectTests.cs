namespace HDF5.Api.Tests
{
    [TestClass]
    public class H5ObjectTests : H5Test<H5ObjectTests>
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

                // Create attribute
                using var att = group.CreateAttribute("att", type, space);
                att.AssertHasHandleType(HandleType.Attribute);

                // Create data set
                using var ds = CreateTestDataset(group, "ds");
                ds.AssertHasHandleType(HandleType.DataSet);
            });
        }

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
        public void DisposeAlreadyDisposedObjectDoesntThrow()
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