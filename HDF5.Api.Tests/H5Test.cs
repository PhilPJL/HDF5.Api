using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;

namespace HDF5.Api.Tests;

[TestClass]
public abstract class H5Test
{
    private const string TestFolder = "TestFiles";

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

    protected static H5File CreateFile2(
        string suffix,
        [CallerMemberName] string? path = null,
        bool failIfExists = false,
        H5PropertyList? fileCreationPropertyList = null)
    {
        Guard.IsNotNull(path);

        return CreateFile($"{path}_{suffix}", failIfExists, fileCreationPropertyList);
    }

    protected static H5File CreateFile(
        [CallerMemberName] string? path = null,
        bool failIfExists = false,
        H5PropertyList? fileCreationPropertyList = null)
    {
        Guard.IsNotNull(path);

        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension(path, "h5"));

        var file = H5File.Create(
            fullpath,
            failIfExists,
            fileCreationPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }

    protected static H5File CreateOrOpenFile(
        [CallerMemberName] string? path = null,
        bool readOnly = false,
        H5PropertyList? fileCreationPropertyList = null)
    {
        Guard.IsNotNull(path);

        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension(path, "h5"));

        var file = H5File.CreateOrOpen(
            fullpath,
            readOnly,
            fileCreationPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }

    protected static H5File OpenFile(
        [CallerMemberName] string? path = null,
        bool readOnly = false,
        H5PropertyList? fileCreationPropertyList = null)
    {
        Guard.IsNotNull(path);

        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension(path, "h5"));

        var file = H5File.Open(
            fullpath,
            readOnly,
            fileCreationPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }
}
