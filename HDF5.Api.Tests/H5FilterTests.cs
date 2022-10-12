using System.ComponentModel;

namespace HDF5.Api.Tests;

[TestClass]
public class H5FilterTests : H5Test
{
    [TestMethod]
    public void IsSZipAvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.SZip)); 
    }

    [TestMethod]
    public void IsDeflateAvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Deflate)); 
    }

    [TestMethod]
    public void IsShuffleAvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Shuffle)); 
    }

    [TestMethod]
    public void IsFletcher32AvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Fletcher32)); 
    }

    [TestMethod]
    public void IsScaleOffsetAvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.ScaleOffset)); 
    }

    [TestMethod]
    public void IsNBitAvailableSucceeds()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.NBit)); 
    }

    [TestMethod]
    public void InvalidTypeThrows()
    {
        Assert.ThrowsException<InvalidEnumArgumentException>(() => H5Filter.IsFilterAvailable(FilterType.None)); 
    }
}