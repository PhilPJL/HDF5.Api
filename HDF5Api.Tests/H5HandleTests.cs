using HDF5Api;

namespace HDF5Api.Tests;

[TestClass]
public class H5HandleTests : H5Test
{
    const string Path = "test.h5";

    [TestMethod]
    public void DetectUnclosedHandlesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create file without disposing
            H5File file = null;
            HandleCheck(() => file = H5File.Create(Path), 1);
            Assert.IsTrue(File.Exists(Path));

            // Now dispose
            file?.Dispose();
        });
    }
}
