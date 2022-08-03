using System.Diagnostics;

namespace HDF5Api.Tests;

[TestClass]
public abstract class H5Test
{
    protected H5Test()
    {
    }

    [TestInitialize]
    public void TestInitialize()
    {
        H5Error.SetAutoOff();
    }

    /// <summary>
    ///     Verify all handles have been closed after completion of test.
    /// </summary>
    /// <param name="action"></param>
    protected static void HandleCheck(Action action, int expectedHandlesOpen = 0)
    {
        action();
#if DEBUG
        if (expectedHandlesOpen == 0 && H5Handle.Handles.Count > 0)
        {
            foreach (var kvp in H5Handle.Handles)
            {
                Debug.WriteLine(kvp.Value);
            }
        }

        Assert.AreEqual(expectedHandlesOpen, H5Handle.Handles.Count);
#else
        Assert.AreEqual(expectedHandlesOpen, H5Handle.OpenHandleCount);
#endif
    }
}
