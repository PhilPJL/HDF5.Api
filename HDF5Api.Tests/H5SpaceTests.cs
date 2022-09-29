namespace HDF5Api.Tests;

[TestClass]
public class H5SpaceTests : H5LocationTests
{
    [TestMethod]
    public void CreateOneDimension()
    {
        using var space = H5Space.Create(new Dimension(5, 100));

        Assert.AreEqual(1, space.GetSimpleExtentNDims());
        Assert.AreEqual(new Dimension(5, 100), space.GetSimpleExtentDims()[0]);
        Assert.AreEqual(5, space.GetSimpleExtentNPoints());
    }

    [TestMethod]
    public void CreateTwoDimensions()
    {
        using var space = H5Space.Create(new Dimension(5, 100), new Dimension(3, 1000));

        Assert.AreEqual(2, space.GetSimpleExtentNDims());
        Assert.AreEqual(new Dimension(5, 100), space.GetSimpleExtentDims()[0]);
        Assert.AreEqual(new Dimension(3, 1000), space.GetSimpleExtentDims()[1]);
        Assert.AreEqual(5 * 3, space.GetSimpleExtentNPoints());
    }

    [TestMethod]
    public void CreateThreeDimensions()
    {
        using var space = H5Space.Create(new Dimension(1, 100), new Dimension(3, 1000), new Dimension(17, 99));

        Assert.AreEqual(3, space.GetSimpleExtentNDims());
        Assert.AreEqual(new Dimension(1, 100), space.GetSimpleExtentDims()[0]);
        Assert.AreEqual(new Dimension(3, 1000), space.GetSimpleExtentDims()[1]);
        Assert.AreEqual(new Dimension(17, 99), space.GetSimpleExtentDims()[2]);
        Assert.AreEqual(1 * 3 * 17, space.GetSimpleExtentNPoints());
    }
}