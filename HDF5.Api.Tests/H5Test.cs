using System.Runtime.CompilerServices;

namespace HDF5.Api.Tests;

[TestClass]
public abstract class H5Test
{
    internal static string TestFolder = "TestFiles";

    [TestInitialize]
    public void TestInitialize()
    {
        H5Error.DisableErrorPrinting();

        if (!Directory.Exists(TestFolder))
        {
            Directory.CreateDirectory(TestFolder);
        }
    }

    /// <summary>
    ///     Verify all handles have been closed after completion of test.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="expectedHandlesOpen">The number of handles expected to be open after the action has run.</param>
    protected static void HandleCheck(Action action, int expectedHandlesOpen = 0)
    {
        action();
#if DEBUG
        if (expectedHandlesOpen == 0)
        {
            H5Handle.DumpOpenHandles();
        }
#endif
        Assert.AreEqual(expectedHandlesOpen, H5Handle.OpenHandleCount);
    }

    protected static H5File CreateFile([CallerMemberName] string? path = null)
    {
        return CreateFile(Path.Combine(TestFolder, Path.ChangeExtension(path, "h5")!)!, false);
    }

    protected static H5File CreateFile(string path,
        bool failIfExists,
        H5PropertyList? fileCreationPropertyList = null,
        H5PropertyList? fileAccessPropertyList = null)
    {
        // Create new file with default property lists
        var file = H5File.Create(path, failIfExists, fileCreationPropertyList, fileAccessPropertyList);
        Assert.IsTrue(File.Exists(path));

        return file;
    }
}
