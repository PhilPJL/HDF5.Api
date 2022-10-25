using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.Tests;

[TestClass]
public class H5EnumTypeTests : H5Test<H5EnumTypeTests>
{
    enum TestInt
    {
        one = 1,
        two = 2,
        max = int.MaxValue,
        min = int.MinValue,
        ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ = 9076
    }

    enum TestInt2
    {
        one = 1,
        two = 2,
        max = int.MaxValue,
        min = int.MinValue,
        ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ = 9076
    }

    enum TestShort : short
    {
        one = -100,
        two = 275,
        max = short.MaxValue,
        min = short.MinValue,
        ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ = 12345
    }

    enum TestByte : byte
    {
        one = 1
    }

    enum TestUInt : uint
    {
        one = 1
    }

    enum TestUShort : ushort
    {
        one = 1
    }

    enum TestULong : ulong
    {
        one = 1
    }

    enum TestLong : long
    {
        one = 1
    }

    [TestMethod]
    public void CreateAndExerciseIntBaseEnumTypeSucceeds()
    {
        HandleCheck((file) =>
        {
            using var enumType = H5TAdapter.CreateBaseEnumType<TestInt>();

            H5TAdapter.InsertEnumMember(enumType, nameof(TestInt.one), TestInt.one);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestInt.two), TestInt.two);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestInt.min), TestInt.min);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestInt.max), TestInt.max);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ), TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ);

            Assert.AreEqual(nameof(TestInt.one), H5TAdapter.NameOfEnumMember(enumType, TestInt.one));
            Assert.AreEqual(nameof(TestInt.two), H5TAdapter.NameOfEnumMember(enumType, TestInt.two));
            Assert.AreEqual(nameof(TestInt.min), H5TAdapter.NameOfEnumMember(enumType, TestInt.min));
            Assert.AreEqual(nameof(TestInt.max), H5TAdapter.NameOfEnumMember(enumType, TestInt.max));
            Assert.AreEqual(nameof(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ), H5TAdapter.NameOfEnumMember(enumType, TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ));

            Assert.AreEqual(TestInt.one, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.one)));
            Assert.AreEqual(TestInt.two, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.two)));
            Assert.AreEqual(TestInt.min, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.min)));
            Assert.AreEqual(TestInt.max, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.max)));
            Assert.AreEqual(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ)));
        });
    }

    [TestMethod]
    public void CreateAndExerciseIntEnumTypeSucceeds()
    {
        HandleCheck((file) =>
        {
            using var enumType = H5Type.CreateEnumType<TestInt>();

            Assert.AreEqual(nameof(TestInt.one), H5TAdapter.NameOfEnumMember(enumType, TestInt.one));
            Assert.AreEqual(nameof(TestInt.two), H5TAdapter.NameOfEnumMember(enumType, TestInt.two));
            Assert.AreEqual(nameof(TestInt.min), H5TAdapter.NameOfEnumMember(enumType, TestInt.min));
            Assert.AreEqual(nameof(TestInt.max), H5TAdapter.NameOfEnumMember(enumType, TestInt.max));
            Assert.AreEqual(nameof(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ), H5TAdapter.NameOfEnumMember(enumType, TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ));

            Assert.AreEqual(TestInt.one, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.one)));
            Assert.AreEqual(TestInt.two, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.two)));
            Assert.AreEqual(TestInt.min, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.min)));
            Assert.AreEqual(TestInt.max, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.max)));
            Assert.AreEqual(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ, H5TAdapter.ValueOfEnumMember<TestInt>(enumType, nameof(TestInt.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ)));
        });
    }
    
    [TestMethod]
    public void IntEnumTypesAreEqual()
    {
        HandleCheck((file) =>
        {
            using var enumType1 = H5Type.CreateEnumType<TestInt>();
            using var enumType2 = H5Type.CreateEnumType<TestInt>();
            using var enumType3 = H5Type.CreateEnumType<TestInt2>();

            Assert.IsTrue(enumType1.Equals(enumType2));
            Assert.IsTrue(enumType1.Equals(enumType3));
        });
    }

    [TestMethod]
    public void CreateAndExerciseShortEnumTypeSucceeds()
    {
        HandleCheck((file) =>
        {
            using var enumType = H5TAdapter.CreateBaseEnumType<TestShort>();

            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.one), TestShort.one);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.two), TestShort.two);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.min), TestShort.min);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.max), TestShort.max);
            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ), TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ);

            Assert.AreEqual(nameof(TestShort.one), H5TAdapter.NameOfEnumMember(enumType, TestShort.one));
            Assert.AreEqual(nameof(TestShort.two), H5TAdapter.NameOfEnumMember(enumType, TestShort.two));
            Assert.AreEqual(nameof(TestShort.min), H5TAdapter.NameOfEnumMember(enumType, TestShort.min));
            Assert.AreEqual(nameof(TestShort.max), H5TAdapter.NameOfEnumMember(enumType, TestShort.max));
            Assert.AreEqual(nameof(TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ), H5TAdapter.NameOfEnumMember(enumType, TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ));

            Assert.AreEqual(TestShort.one, H5TAdapter.ValueOfEnumMember<TestShort>(enumType, nameof(TestShort.one)));
            Assert.AreEqual(TestShort.two, H5TAdapter.ValueOfEnumMember<TestShort>(enumType, nameof(TestShort.two)));
            Assert.AreEqual(TestShort.min, H5TAdapter.ValueOfEnumMember<TestShort>(enumType, nameof(TestShort.min)));
            Assert.AreEqual(TestShort.max, H5TAdapter.ValueOfEnumMember<TestShort>(enumType, nameof(TestShort.max)));
            Assert.AreEqual(TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ, H5TAdapter.ValueOfEnumMember<TestShort>(enumType, nameof(TestShort.ᚢᛗᛋᚳᛖᚪᛚᚦᛖᚪᚻ)));
        });
    }

    [TestMethod]
    public void CreateIntEnumTypesSucceeds()
    {
        HandleCheck((file) =>
        {
            using var e1 = H5TAdapter.CreateBaseEnumType<TestByte>();
            using var e2 = H5TAdapter.CreateBaseEnumType<TestShort>();
            using var e3 = H5TAdapter.CreateBaseEnumType<TestUShort>();
            using var e4 = H5TAdapter.CreateBaseEnumType<TestInt>();
            using var e5 = H5TAdapter.CreateBaseEnumType<TestUInt>();
            using var e6 = H5TAdapter.CreateBaseEnumType<TestLong>();
            using var e7 = H5TAdapter.CreateBaseEnumType<TestULong>();
        });
    }

    [TestMethod]
    public void CreateDuplicateEnumNameFails()
    {
        HandleCheck((file) =>
        {
            using var enumType = H5TAdapter.CreateBaseEnumType<TestShort>();

            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.one), TestShort.one);
            Assert.ThrowsException<H5Exception>(() =>
                H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.one), TestShort.two));
        });
    }

    [TestMethod]
    public void CreateDuplicateEnumValueFails()
    {
        HandleCheck((file) =>
        {
            using var enumType = H5TAdapter.CreateBaseEnumType<TestShort>();

            H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.one), TestShort.one);
            Assert.ThrowsException<H5Exception>(() =>
                H5TAdapter.InsertEnumMember(enumType, nameof(TestShort.two), TestShort.one));
        });
    }
}