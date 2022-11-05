using System.Runtime.InteropServices;
using HDF5.Api.H5Types;

namespace HDF5.Api.Tests;

[TestClass]
public class H5TypeTests : H5Test<H5TypeTests>
{
    [TestMethod]
    public void CreateCommittedDataTypeAsciiSucceeds()
    {
        HandleCheck((file) =>
        {
            const string bigString = "bigstring";

            using var bigStringType = H5StringType.CreateFixedLengthStringType(1000);
            file.Commit(bigString, bigStringType);

            Assert.IsTrue(file.DataTypeNames.Contains(bigString));

            const string smallString = "smallstring";

            using var group = file.CreateGroup("group");
            using var smallStringType = H5StringType.CreateFixedLengthStringType(10);
            group.Commit(smallString, smallStringType);

            Assert.IsTrue(group.DataTypeNames.Contains(smallString));
        });
    }

    [TestMethod]
    public void CreateCommittedDataTypeUtf8Succeeds()
    {
        HandleCheck((file) =>
        {
            const string bigString = "Χαρακτηριστικό";

            using var bigStringType = H5StringType.CreateFixedLengthStringType(1000);
            file.Commit(bigString, bigStringType);

            Assert.IsTrue(file.DataTypeNames.Contains(bigString));

            const string smallString = "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ";

            using var group = file.CreateGroup("group");
            using var smallStringType = H5StringType.CreateFixedLengthStringType(10);
            group.Commit(smallString, smallStringType);

            Assert.IsTrue(group.DataTypeNames.Contains(smallString));
        });
    }

    [TestMethod]
    public void CreateEnumerateCompoundDataTypeSucceeds()
    {
        HandleCheck((file) =>
        {
            using var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό));

            Assert.AreEqual(3, type.NumberOfMembers);

            // TODO: enumerate/get member/etc
        });
    }

    [TestMethod]
    [DataRow("TypeWithUtf8PropertyAndAsciiName")]
    [DataRow("TypeWithUtf8PropertyAndUf8Name_᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫")]
    public void CreateAndOpenCompoundDataTypeAsciiSucceeds(string name)
    {
        HandleCheck2((file) =>
        {
            using (var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό)))
            {
                file.Commit(name, type);
            }

            using (var type = file.OpenType(name))
            {
                Assert.AreEqual("/" + name, type.Name);
            }
        },
        name);
    }

    [TestMethod]
    public void OpenInvalidCompoundDataTypeFails()
    {
        HandleCheck((file) =>
        {
            Assert.ThrowsException<H5Exception>(() => file.OpenType("TypeWithUtf8"));
        });

    }

    [TestMethod]
    public void NativeTypes()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            using var type = H5Type.GetEquivalentNativeType<int>();
            using var type2 = H5Type.GetEquivalentNativeType(type);

            Assert.IsTrue(type.IsEqualTo(type2));
        });
    }

    #region Attributes

    [TestMethod]
    public void CreateWriteReadDeleteAttributesSucceeds()
    {
        HandleCheck((file) =>
        {
            using var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό));

            file.Commit("TypeWithUtf8", type);

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(type);
        });
    }

    [TestMethod]
    public void AttributeOnUncommittedTypeThrows()
    {
        HandleCheck((file) =>
        {
            using var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό));

            Assert.ThrowsException<H5Exception>(() => type.CreateAndWriteAttribute("test", 1));
        });
    }

    [TestMethod]
    public void CreateIterateAttributesSucceeds()
    {
        HandleCheck((file) =>
        {
            using var type = H5Type.CreateCompoundType<CompoundType>()
                .Insert<CompoundType, int>(nameof(CompoundType.Id))
                .Insert<CompoundType, short>(nameof(CompoundType.ShortProperty))
                .Insert<CompoundType, long>(nameof(CompoundType.Χαρακτηριστικό));

            file.Commit("TypeWithUtf8", type);

            H5AttributeTests.CreateIterateAttributesSucceeds(type);
        });
    }

    #endregion

    [StructLayout(LayoutKind.Sequential)]
    private struct CompoundType
    {
        public int Id;
        public short ShortProperty;
        public long Χαρακτηριστικό;
    }
}
