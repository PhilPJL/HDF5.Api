﻿using System.Diagnostics;

namespace HDF5.Api.Tests;

[TestClass]
public class H5GlobalTests : H5Test<H5GlobalTests>
{
    [TestMethod]
    public void IsThreadSafe()
    {
        Assert.IsTrue(H5Global.IsThreadSafe());
    }

    [TestMethod]
    public void IsVersion1_10_0_6()
    {
        Assert.AreEqual(new Version(1, 10, 0, 6), H5Global.GetLibraryVersion());
    }

    [TestMethod]
    public void DescribeSucceeds()
    {
        string description = H5Global.Describe();

        Assert.IsNotNull(description);

        Debug.WriteLine(description);
    }
}
