namespace HDF5Api.Tests;

[TestClass]
public class H5HandleTests : H5Test
{
    private const string Path = "test.h5";

    [TestMethod]
    public void DetectUnclosedHandlesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create file without disposing
            H5File? file = null;
            HandleCheck(() => file = H5File.Create(Path), 1);
            Assert.IsTrue(File.Exists(Path));

#if DEBUG
            H5Handle.DumpOpenHandles();
#endif
            
            // Now dispose
            file?.Dispose();
        });
    }
}
