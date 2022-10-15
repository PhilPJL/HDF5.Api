﻿namespace HDF5.Api.Tests;

[TestClass]
public abstract class H5Test
{
    [TestInitialize]
    public void TestInitialize()
    {
        H5Error.SetAutoOff();
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

    protected static H5File CreateFile(string path,
        bool failIfExists = false,
        H5PropertyList? fileCreationPropertyList = null,
        H5PropertyList? fileAccessPropertyList = null)
    {
        File.Delete(path);
        Assert.IsFalse(File.Exists(path));

        // Create new file with default property lists
        var file = H5File.Create(path, failIfExists, fileCreationPropertyList, fileAccessPropertyList);
        Assert.IsTrue(File.Exists(path));

        return file;
    }
}
