namespace HDF5.Api.Tests;

[TestClass]
public class GeneralTests : H5Test<H5AttributeTests>
{
    [TestMethod]
    public void InvalidHandleThrows()
    {
        Assert.ThrowsException<H5Exception>(() => long.MinValue.ThrowIfInvalidHandleValue());
        Assert.ThrowsException<H5Exception>(() => (-1L).ThrowIfInvalidHandleValue());
        Assert.ThrowsException<H5Exception>(() => (-2L).ThrowIfInvalidHandleValue("  "));

        // Doesn't throw
        0L.ThrowIfInvalidHandleValue();
        long.MaxValue.ThrowIfInvalidHandleValue();
    }

    [TestMethod]
    public void DefaultHandleThrows()
    {
        Assert.ThrowsException<H5Exception>(() => long.MinValue.ThrowIfDefaultOrInvalidHandleValue("bad"));
        Assert.ThrowsException<H5Exception>(() => (-1L).ThrowIfDefaultOrInvalidHandleValue("bad2"));
        Assert.ThrowsException<H5Exception>(() => 0L.ThrowIfDefaultOrInvalidHandleValue("bad"));
        Assert.ThrowsException<H5Exception>(() => 0L.ThrowIfDefaultOrInvalidHandleValue("  "));

        // Doesn't throw
        1L.ThrowIfDefaultOrInvalidHandleValue("good");
        long.MaxValue.ThrowIfDefaultOrInvalidHandleValue("good");
    }

    [TestMethod]
    public void ErrorValueThrows()
    {
        Assert.ThrowsException<H5Exception>(() => (-1L).ThrowIfError());
        Assert.ThrowsException<H5Exception>(() => (-1L).ThrowIfError("  "));
        Assert.ThrowsException<H5Exception>(() => (-1).ThrowIfError("test"));
        Assert.ThrowsException<H5Exception>(() => (-1).ThrowIfError("  "));
        Assert.ThrowsException<H5Exception>(() => long.MinValue.ThrowIfError());
        Assert.ThrowsException<H5Exception>(() => int.MinValue.ThrowIfError());

        0L.ThrowIfError();
        0.ThrowIfError();
        long.MaxValue.ThrowIfError();
        int.MaxValue.ThrowIfError();
    }

    [TestMethod]
    public void CheckBenchmarkCodeSucceeds()
    {
        HandleCheck((file) =>
        {
            var dt = DateTime.UtcNow;
            var shortString = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            var longString = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";

            file.WriteAttribute("DateTime", dt);
            file.WriteAttribute("shortstring", shortString, 101);
            file.WriteAttribute("longstring", longString, 1001);

            var dt2 = file.ReadAttribute<DateTime>("DateTime");
            Assert.AreEqual(dt, dt2);

            var shortString2 = file.ReadAttribute<string>("shortstring");
            Assert.AreEqual(shortString, shortString2);

            var longString2 = file.ReadAttribute<string>("longstring");
            Assert.AreEqual(longString, longString2);
        });
    }
}