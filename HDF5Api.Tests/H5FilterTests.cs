namespace HDF5Api.Tests;

[TestClass]
public class H5FilterTests : H5Test
{
    [TestMethod]
    public void IsSZipAvailable()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.SZip)); 
    }

    [TestMethod]
    public void IsDeflateAvailable()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Deflate)); 
    }

    [TestMethod]
    public void IsShuffleAvailable()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Shuffle)); 
    }

    [TestMethod]
    public void IsFletcher32Available()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.Fletcher32)); 
    }

    [TestMethod]
    public void IsScaleOffsetAvailable()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.ScaleOffset)); 
    }

    [TestMethod]
    public void IsNBitAvailable()
    {
        Assert.IsTrue(H5Filter.IsFilterAvailable(FilterType.NBit)); 
    }
}