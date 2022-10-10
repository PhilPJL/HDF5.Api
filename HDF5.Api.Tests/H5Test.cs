namespace HDF5.Api.Tests;

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
}
