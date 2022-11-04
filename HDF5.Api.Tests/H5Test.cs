using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;

namespace HDF5.Api.Tests;

[TestClass]
public abstract class H5Test<T> where T : H5Test<T>
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

        // TODO: verify H5 allocated memory has been released
    }

    /// <summary>
    ///     Verify all handles have been closed after completion of test.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="expectedHandlesOpen">The number of handles expected to be open after the action has run.</param>
    internal static void HandleCheck(Action<H5File> action,
        [CallerMemberName] string? path = null, bool failIfExists = false, H5FileCreationPropertyList? fileCreationPropertyList = null,
        int expectedHandlesOpen = 0)
    {
        using var file = CreateFile(path, failIfExists, fileCreationPropertyList);

        action(file);

#if DEBUG
        if (expectedHandlesOpen == 0 && H5Handle.OpenHandleCount > 1)
        {
            H5Handle.DumpOpenHandles();
        }
#endif

        Assert.AreEqual(file.GetObjectCount(), 1);
        Assert.AreEqual(file.GetObjectCount(), H5Handle.OpenHandleCount);
        Assert.AreEqual(expectedHandlesOpen + 1, H5Handle.OpenHandleCount);

        // TODO: verify H5 allocated memory has been released
    }

    /// <summary>
    ///     Verify all handles have been closed after completion of test.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="expectedHandlesOpen">The number of handles expected to be open after the action has run.</param>
    internal static void HandleCheck2(Action<H5File> action,
        string suffix,
        [CallerMemberName] string? path = null, bool failIfExists = false,
        H5FileCreationPropertyList? fileCreationPropertyList = null,
        H5FileAccessPropertyList? fileAccessPropertyList = null,
        int expectedHandlesOpen = 0,
        int expectedFileObjectCount = 1)
    {
        using var file = CreateFile2(suffix, path, failIfExists, fileCreationPropertyList, fileAccessPropertyList);

        action(file);

#if DEBUG
        if (expectedHandlesOpen == 0 && H5Handle.OpenHandleCount > 1)
        {
            H5Handle.DumpOpenHandles();
        }
#endif
        Assert.AreEqual(file.GetObjectCount(), expectedFileObjectCount);

        // file.GetObjectCount() doesn't always match H5Handle.OpenHandleCount
        // if object created outside of HandleCheck
        //Assert.AreEqual(file.GetObjectCount(), H5Handle.OpenHandleCount);

        Assert.AreEqual(expectedHandlesOpen + 1, H5Handle.OpenHandleCount);

        // TODO: verify H5 allocated memory has been released
    }

    protected string GetFileName([CallerMemberName] string? path = null)
    {
        return Path.Combine(TestFolder, Path.ChangeExtension($"{typeof(T).Name}-{path}", "h5"));
    }

    protected string GetFileName2(string suffix, [CallerMemberName] string? path = null)
    {
        return GetFileName($"{path}_{suffix}");
    }

    internal static H5File CreateFile2(
        string suffix,
        [CallerMemberName] string? path = null,
        bool failIfExists = false,
        H5FileCreationPropertyList? fileCreationPropertyList = null,
        H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNull(path);

        return CreateFile($"{path}_{suffix}", failIfExists, fileCreationPropertyList, fileAccessPropertyList);
    }

    internal static H5File CreateFile(
        [CallerMemberName] string? path = null,
        bool failIfExists = false,
        H5FileCreationPropertyList? fileCreationPropertyList = null,
        H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNull(path);

        var typeName = typeof(T).Name;
        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension($"{typeName}-{path}", "h5"));

        var file = H5File.Create(
            fullpath,
            failIfExists,
            fileCreationPropertyList,
            fileAccessPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }

    internal static H5File CreateOrOpenFile(
        [CallerMemberName] string? path = null,
        bool readOnly = false,
        H5FileCreationPropertyList? fileCreationPropertyList = null,
        H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNull(path);

        var typeName = typeof(T).Name;
        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension($"{typeName}-{path}", "h5"));

        var file = H5File.CreateOrOpen(
            fullpath,
            readOnly,
            fileCreationPropertyList,
            fileAccessPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }

    internal static H5File OpenFile(
        [CallerMemberName] string? path = null,
        bool readOnly = false,
        H5FileAccessPropertyList? fileAccessPropertyList = null)
    {
        Guard.IsNotNull(path);

        var typeName = typeof(T).Name;
        var fullpath = Path.Combine(TestFolder, Path.ChangeExtension($"{typeName}-{path}", "h5"));

        var file = H5File.Open(
            fullpath,
            readOnly,
            fileAccessPropertyList);

        Assert.IsTrue(File.Exists(fullpath));

        return file;
    }
}
