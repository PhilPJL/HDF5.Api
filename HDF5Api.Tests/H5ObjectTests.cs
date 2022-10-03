namespace HDF5Api.Tests
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
            });
        }

        [TestMethod]
        public void DisposeAlreadyDisposedObjectThrowsTest()
        {
            HandleCheck(() =>
            {
                // Ensure no existing file
                File.Delete(Path);
                Assert.IsFalse(File.Exists(Path));

                // Create new file
                var file = H5File.Create(Path);
                Assert.IsFalse(file.IsDisposed());
                file.Dispose();
                Assert.IsTrue(file.IsDisposed());
                Assert.ThrowsException<ObjectDisposedException>(() => file.Dispose());
            });
        }
    }
}