using System.Runtime.InteropServices;

namespace HDF5.Api.Tests;

[TestClass]
public class H5TypeTests : H5Test<H5TypeTests> 
{
    [TestMethod]
    public void CreateCommittedDataTypeAsciiSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            const string bigString = "bigstring";

            using var bigStringType = H5Type.CreateFixedLengthStringType(1000);
            file.Commit(bigString, bigStringType);

            Assert.IsTrue(file.NamedDataTypeNames.Contains(bigString));

            const string smallString = "smallstring";

            using var group = file.CreateGroup("group");
            using var smallStringType = H5Type.CreateFixedLengthStringType(10);
            group.Commit(smallString, smallStringType);

            Assert.IsTrue(group.NamedDataTypeNames.Contains(smallString));
        });
    }

    [TestMethod]
    public void CreateCommittedDataTypeUtf8Succeeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            const string bigString = "Χαρακτηριστικό";

            using var bigStringType = H5Type.CreateFixedLengthStringType(1000);
            file.Commit(bigString, bigStringType);

            Assert.IsTrue(file.NamedDataTypeNames.Contains(bigString));

            const string smallString = "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ";

            using var group = file.CreateGroup("group");
            using var smallStringType = H5Type.CreateFixedLengthStringType(10);
            group.Commit(smallString, smallStringType);

            Assert.IsTrue(group.NamedDataTypeNames.Contains(smallString));
        });
    }

    [TestMethod]
    public void CreateCompoundDataTypeSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            using var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό));

            file.Commit("TypeWithUtf8", type);
        });
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CompoundType
    {
        public int Id;
        public short ShortProperty;
        public long Χαρακτηριστικό;
    }
}