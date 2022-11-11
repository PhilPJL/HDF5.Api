using System.Diagnostics;

namespace HDF5.Api.Tests;

[TestClass]
public class H5AttributeTests : H5Test<H5AttributeTests>
{
    [TestMethod]
    [DataRow("ascii_nullterm", CharacterSet.Ascii, StringPadding.NullTerminate)]
    [DataRow("ascii_nullpad", CharacterSet.Ascii, StringPadding.NullPad)]
    [DataRow("ascii_nullspace", CharacterSet.Ascii, StringPadding.Space)]
    [DataRow("utf8_nullterm", CharacterSet.Utf8, StringPadding.NullTerminate)]
    [DataRow("utf8_nullpad", CharacterSet.Utf8, StringPadding.NullPad)]
    [DataRow("utf8_space", CharacterSet.Utf8, StringPadding.Space)]
    public void ReadWriteFixedLengthStringAttributeSucceeds(string fileNameSuffix, CharacterSet characterSet, StringPadding padding)
    {
        HandleCheck2((file) =>
        {
            //Test(file, "fixed_null", null!, 10, characterSet, padding); // will throw
            Test(file, "fixed_empty", "", 10, characterSet, padding);
            Test(file, "fixed_22", "12345678912345678912", 32, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "fixed_500", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 500, characterSet, padding);
            }

            static void Test<T>(H5ObjectWithAttributes<T> file, string name, string value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
                 where T : H5ObjectWithAttributes<T>
            {
                file.WriteAttribute(name, value, fixedStorageLength, characterSet, padding);

                using var attribute = file.OpenStringAttribute(name);
                var r = attribute.Read();

                // NOTE: assuming that if no value is written ReadString will return string.Empty
                Assert.AreEqual(value ?? string.Empty, r);
                Assert.AreEqual(name, attribute.Name);
            }
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("v_ascii_nullterm", CharacterSet.Ascii, StringPadding.NullTerminate)]
    [DataRow("v_ascii_nullpad", CharacterSet.Ascii, StringPadding.NullPad)]
    [DataRow("v_ascii_nullspace", CharacterSet.Ascii, StringPadding.Space)]
    [DataRow("v_utf8_nullterm", CharacterSet.Utf8, StringPadding.NullTerminate)]
    [DataRow("v_utf8_nullpad", CharacterSet.Utf8, StringPadding.NullPad)]
    [DataRow("v_utf8_space", CharacterSet.Utf8, StringPadding.Space)]
    public void ReadWriteVariableLengthStringAttributeSucceeds(string fileNameSuffix, CharacterSet characterSet, StringPadding padding)
    {
        HandleCheck2((file) =>
        {
            //Test(file, "variable_null", null, 0, characterSet, padding);
            Test(file, "variable_empty", "", 0, characterSet, padding);
            Test(file, "variable_short", "12345678912345678912", 0, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "variable_long", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 0, characterSet, padding);
            }

            static void Test<T>(H5ObjectWithAttributes<T> file, string name, string value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
                where T : H5ObjectWithAttributes<T>
            {
                file.WriteAttribute(name, value, fixedStorageLength, characterSet, padding);

                using var attribute = file.OpenStringAttribute(name);
                var r = attribute.Read();

                // NOTE: assuming that if no value is written ReadString will return string.Empty
                Assert.AreEqual(value ?? string.Empty, r);

                Assert.AreEqual(name, attribute.Name);
            }
        },
        fileNameSuffix);
    }

    [TestMethod]
    [DataRow("ascii", "an ascii attribute name")]
    [DataRow("utf8", "Χαρακτηριστικό")]
    public void CreateDuplicateAttributeNameThrows(string filename, string attributeName)
    {
        HandleCheck2((file) =>
        {
            // Create group
            using var group = file.CreateGroup("group");

            // Create duplicate attribute on file
            file.WriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => file.WriteAttribute(attributeName, 2, writeBehaviour: AttributeWriteBehaviour.ThrowIfAlreadyExists));

            // Create duplicate attribute on group
            group.WriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => group.WriteAttribute(attributeName, 2, writeBehaviour: AttributeWriteBehaviour.ThrowIfAlreadyExists));

            // Create test dataset on file
            using var datasetFile = H5DataSetTests.CreateTestDataset(file, "aDataSet");

            // Create duplicate attribute on data set on file
            datasetFile.WriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => datasetFile.WriteAttribute(attributeName, 2, writeBehaviour: AttributeWriteBehaviour.ThrowIfAlreadyExists));

            // Create test dataset on group
            using var datasetGroup = H5DataSetTests.CreateTestDataset(group, "aDataSet");

            // Create duplicate attribute on data set on group
            datasetGroup.WriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => datasetGroup.WriteAttribute(attributeName, 2, writeBehaviour: AttributeWriteBehaviour.ThrowIfAlreadyExists));

            // File + Group + 2 x DataSet
            Assert.AreEqual(4, file.GetObjectCount());
        },
        filename);
    }

    [TestMethod]
    public void RewriteStringAttributeSucceeds()
    {
        HandleCheck((file) =>
        {
            // Create group
            using var group = file.CreateGroup("group");

            group.WriteAttribute("int", 1);
            group.WriteAttribute("string", "short", 10);
            group.WriteAttribute("long", 1L);

            group.DeleteAttribute("string");

            const string s = "long-----------------------------------";
            group.WriteAttribute("string", s, 100);

            string s1 = group.ReadAttribute<string>("string");
            Assert.AreEqual(s, s1);

            // File + Group
            Assert.AreEqual(2, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void ReadInvalidAttributeTypeFails()
    {
        HandleCheck((file) =>
        {
            // Create group
            using var group = file.CreateGroup("group");

            group.WriteAttribute("int", 1);
            group.WriteAttribute("string", "short", 10);

            string s1 = group.ReadAttribute<string>("string");
            Assert.AreEqual("short", s1);

            int i1 = group.ReadAttribute<int>("int");
            Assert.AreEqual(1, i1);

            Assert.ThrowsException<H5Exception>(() => group.ReadAttribute<string>("int"));
            Assert.ThrowsException<H5Exception>(() => group.ReadAttribute<int>("string"));
        });
    }

    [TestMethod]
    public void ReadingMismatchedEnumTypeThrows()
    {
        HandleCheck((file) =>
        {
            file.WriteAttribute("test-ulong.min", TestULong.Min);

            var valueTestUlongMin = file.ReadAttribute<TestULong>("test-ulong.min");
            Assert.AreEqual(valueTestUlongMin, TestULong.Min);

            // mismatched type should throw
            Assert.ThrowsException<H5Exception>(() => file.ReadAttribute<TestLong>("test-ulong.min"));
        });
    }

    // Helper methods
    internal static void CreateIterateAttributesSucceeds<T>(H5ObjectWithAttributes<T> location) where T : H5ObjectWithAttributes<T>
    {
        location.WriteAttribute("string", "This is a string 12345.", 50);

        location.WriteAttribute("dateTime-min", DateTime.MinValue);
        location.WriteAttribute("dateTime", DateTime.Now);
        location.WriteAttribute("dateTime-max", DateTime.MaxValue);

        location.WriteAttribute("dateTimeOffset-min", DateTimeOffset.MinValue);
        location.WriteAttribute("dateTimeOffset", DateTimeOffset.Now);
        location.WriteAttribute("dateTimeOffset-max", DateTimeOffset.MaxValue);

        location.WriteAttribute("timeSpan-min", TimeSpan.MinValue);
        location.WriteAttribute("timeSpan-max", TimeSpan.MaxValue);

#if NET7_0_OR_GREATER
        location.WriteAttribute("timeOnly-min", TimeOnly.MinValue);
        location.WriteAttribute("timeOnly-max", TimeOnly.MaxValue);
        location.WriteAttribute("dateOnly-min", DateOnly.MinValue);
        location.WriteAttribute("dateOnly-max", DateOnly.MaxValue);
#endif

        location.WriteAttribute("bool-true", true);
        location.WriteAttribute("bool-false", false);

        location.WriteAttribute("char-min", char.MinValue);
        location.WriteAttribute("char-max", char.MaxValue);

        location.WriteAttribute("byte-min", byte.MinValue);
        location.WriteAttribute("byte", (byte)5);
        location.WriteAttribute("byte-max", byte.MaxValue);

        location.WriteAttribute("int16-min", short.MinValue);
        location.WriteAttribute("int16", (short)5);
        location.WriteAttribute("int16-max", short.MaxValue);

        location.WriteAttribute("uint16-min", ushort.MinValue);
        location.WriteAttribute("uint16", (ushort)6);
        location.WriteAttribute("uint16-max", ushort.MaxValue);

        location.WriteAttribute("int32-min", int.MinValue);
        location.WriteAttribute("int32", 7);
        location.WriteAttribute("int32-max", int.MaxValue);

        location.WriteAttribute("uint32-min", uint.MinValue);
        location.WriteAttribute("uint32", 8u);
        location.WriteAttribute("uint32-max", uint.MaxValue);

        location.WriteAttribute("long-min", long.MinValue);
        location.WriteAttribute("long", 9L);
        location.WriteAttribute("long-max", long.MaxValue);

        location.WriteAttribute("ulong-min", ulong.MinValue);
        location.WriteAttribute("ulong", 10ul);
        location.WriteAttribute("ulong-max", ulong.MaxValue);

        location.WriteAttribute("float-min", float.MinValue);
        location.WriteAttribute("float", 11.0f);
        location.WriteAttribute("float-max", float.MaxValue);

        location.WriteAttribute("double-min", double.MinValue);
        location.WriteAttribute("double", 12.0d);
        location.WriteAttribute("double-max", double.MaxValue);

        location.WriteAttribute("decimal-min", decimal.MinValue);
        location.WriteAttribute("decimal", 12.0m);
        location.WriteAttribute("decimal-max", decimal.MaxValue);

        location.WriteAttribute("enumlong-min", TestLong.Min);
        location.WriteAttribute("enumlong", TestLong.None);
        location.WriteAttribute("enumlong-max", TestLong.Max);

        // TODO: decimal

        var names = new List<string> {
            "char-min",
            "char-max",
            "dateTime-min",
            "dateTime-max",
            "dateTime",
            "dateTimeOffset-min",
            "dateTimeOffset-max",
            "dateTimeOffset",
            "timeSpan-min",
            "timeSpan-max",
#if NET7_0_OR_GREATER
            "timeOnly-min",
            "timeOnly-max",
            "dateOnly-min",
            "dateOnly-max",
#endif
            "bool-true",
            "bool-false",
            "byte-min",
            "byte",
            "byte-max",
            "string",
            "int16-min",
            "int16",
            "int16-max",
            "uint16-min",
            "uint16",
            "uint16-max",
            "int32-min",
            "int32",
            "int32-max",
            "uint32-min",
            "uint32",
            "uint32-max",
            "long-min",
            "long",
            "long-max",
            "ulong-min",
            "ulong",
            "ulong-max",
            "float-min",
            "float",
            "float-max",
            "double-min",
            "double",
            "double-max",
            "decimal-min",
            "decimal",
            "decimal-max",
            "enumlong-min",
            "enumlong",
            "enumlong-max"
        }
        .OrderBy(a => a)
        .ToList();

        var attributeNames = location.AttributeNames.OrderBy(a => a).ToList();

        Assert.IsTrue(names.SequenceEqual(attributeNames));
    }

    internal static void CreateWriteReadDeleteAttributesSucceeds<T>(H5ObjectWithAttributes<T> location) where T : H5ObjectWithAttributes<T>
    {
        // Create attributes
        CreateWriteReadUpdateDeleteAttribute('a', 'b');
        CreateWriteReadUpdateDeleteAttribute('a', 'Â');
        CreateWriteReadUpdateDeleteAttribute((short)15, (short)99);
        CreateWriteReadUpdateDeleteAttribute((ushort)15, (ushort)77);
        CreateWriteReadUpdateDeleteAttribute((byte)15, (byte)0xff);
        CreateWriteReadUpdateDeleteBoolAttribute(true, false);
        CreateWriteReadUpdateDeleteAttribute(15, 1234);
        CreateWriteReadUpdateDeleteAttribute(15u, 4567u);
        CreateWriteReadUpdateDeleteAttribute(15L, long.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(15uL, ulong.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(1.5f, 123.456);
        CreateWriteReadUpdateDeleteAttribute(1.5123d, double.MaxValue);
        CreateWriteReadUpdateDeleteDecimalAttribute(12345678.123m, -9999999.1234m);
        CreateWriteReadUpdateDeleteStringAttribute("1234567890", "ABCDEFGH", 11);
        CreateWriteReadUpdateDeleteStringAttribute("", "ABCDEFGH", 10);
        CreateWriteReadUpdateDeleteStringAttribute("abcdefghijklmnopqrstuvwzyz", "", 27);
        CreateWriteReadUpdateDeleteTimeSpanAttribute(TimeSpan.FromMinutes(1), TimeSpan.FromHours(2.34));
        CreateWriteReadUpdateDeleteTimeSpanAttribute(TimeSpan.FromDays(5.61), TimeSpan.FromMilliseconds(16.345));
#if NET7_0_OR_GREATER
        CreateWriteReadUpdateDeleteTimeOnlyAttribute(TimeOnly.MinValue, TimeOnly.MaxValue);
        CreateWriteReadUpdateDeleteDateOnlyAttribute(DateOnly.MinValue, DateOnly.MaxValue);
#endif
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddYears(5));
        CreateWriteReadUpdateDeleteDateTimeOffsetAttribute(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5));
        CreateWriteReadUpdateDeleteDateTimeOffsetAttribute(DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(-5));
        CreateWriteReadUpdateDeleteStringAttribute(new string('A', 1000), new string('B', 500), 1200);
        CreateWriteReadUpdateDeleteEnumAttribute(TestLong.Min, TestLong.Max);
        CreateWriteReadUpdateDeleteEnumAttribute(TestByte.Min, TestByte.Max);
        Assert.AreEqual(0, location.NumberOfAttributes);

#if NET7_0_OR_GREATER
        void CreateWriteReadUpdateDeleteTimeOnlyAttribute(TimeOnly value, TimeOnly newValue)
        {
            const string name = "dtTimeOnly";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenTimeOnlyAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<TimeOnly>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<TimeOnly>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteDateOnlyAttribute(DateOnly value, DateOnly newValue)
        {
            const string name = "dtDateOnly";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenDateOnlyAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<DateOnly>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<DateOnly>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }
#endif

        void CreateWriteReadUpdateDeleteTimeSpanAttribute(TimeSpan value, TimeSpan newValue)
        {
            const string name = "dtTimeSpan";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenTimeSpanAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<TimeSpan>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<TimeSpan>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime value, DateTime newValue)
        {
            const string name = "dtDateTime";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenDateTimeAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<DateTime>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<DateTime>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteDateTimeOffsetAttribute(DateTimeOffset value, DateTimeOffset newValue)
        {
            const string name = "dtDateTimeOffset";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenDateTimeOffsetAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<DateTimeOffset>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<DateTimeOffset>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteStringAttribute(string value, string newValue, int fixedStorageLength = 0)
        {
            const string name = "dtString";

            location.WriteAttribute(name, value, fixedStorageLength);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenStringAttribute(name);

            Assert.AreEqual(value, location.ReadAttribute<string>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);

            Assert.AreEqual(newValue, location.ReadAttribute<string>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteAttribute<TValue>(TValue value, TValue newValue) where TValue : unmanaged, IEquatable<TValue>
        {
            string name = $"dt{typeof(TValue).Name}";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenPrimitiveAttribute<TValue>(name);
            Assert.AreEqual(value, location.ReadAttribute<TValue>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);
            Assert.AreEqual(newValue, location.ReadAttribute<TValue>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteDecimalAttribute(decimal value, decimal newValue)
        {
            string name = $"dt{typeof(decimal).Name}";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenDecimalAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<decimal>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);
            Assert.AreEqual(newValue, location.ReadAttribute<decimal>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteEnumAttribute<TValue>(TValue value, TValue newValue) where TValue : unmanaged, Enum
        {
            string name = $"dt{typeof(TValue).Name}";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenEnumAttribute<TValue>(name);
            Assert.AreEqual(value, location.ReadAttribute<TValue>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);
            Assert.AreEqual(newValue, location.ReadAttribute<TValue>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteBoolAttribute(bool value, bool newValue)
        {
            string name = $"dt{typeof(bool).Name}";

            location.WriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            using var a = location.OpenBooleanAttribute(name);
            Assert.AreEqual(value, location.ReadAttribute<bool>(name));
            Assert.AreEqual(value, a.Read());

            a.Write(newValue);
            Assert.AreEqual(newValue, location.ReadAttribute<bool>(name));
            Assert.AreEqual(newValue, a.Read());

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }
    }

    [TestMethod]
    [DataRow("ascii_ascii", "an ascii attribute name", "an ascii attribute value", CharacterSet.Ascii)]
    [DataRow("utf8_ascii", "Χαρακτηριστικό", "an ascii attribute value", CharacterSet.Ascii)]
    [DataRow("ascii_utf8", "an ascii attribute name", "ᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪ", CharacterSet.Utf8)]
    [DataRow("utf8_utf8", "Χαρακτηριστικό", "ᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪ", CharacterSet.Utf8)]
    [DataRow("ascii_utf8_long", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.", "ᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", CharacterSet.Utf8)]
    [DataRow("utf8_utf8_long", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬᛫ᚦᛖᚪᚻ᛫ᛗᚪ", CharacterSet.Utf8)]
    public void CreateAttributeSucceeds(string fileSuffix, string attributeName, string value, CharacterSet encoding)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileSuffix);
            file.WriteAttribute(attributeName, value, value.Length * 4, encoding);

            using var attribute = file.OpenStringAttribute(attributeName);
            var read = attribute.Read();

            Assert.AreEqual(value, read);

            Assert.IsTrue(file.AttributeExists(attributeName));
            file.AttributeNames.ForEach(a => Debug.WriteLine(a));

            Assert.AreEqual(attributeName, attribute.Name);
        });
    }

    [TestMethod]
    public void DuplicateAttributeThrowsH5ErrorInfo()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();
            file.WriteAttribute("duplicate", "", 100);

            try
            {
                file.WriteAttribute("duplicate", "", 100, writeBehaviour: AttributeWriteBehaviour.ThrowIfAlreadyExists);
            }
            catch (H5Exception ex)
            {
                Assert.IsNull(ex.InnerException);

                string msg = ex.Message;
                string toString = ex.ToString();
                Debug.WriteLine(msg);
                Debug.WriteLine(toString);

                Assert.AreEqual("Attribute 'duplicate' already exists.", msg);
            }
        });
    }

    [TestMethod]
    public void BoolAttribute()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            file.WriteAttribute("boolTestTrue", true);
            using var attributeTrue = file.OpenBooleanAttribute("boolTestTrue");
            Assert.IsTrue(attributeTrue.Read());
            attributeTrue.Write(false);
            Assert.IsFalse(attributeTrue.Read());

            file.WriteAttribute("boolTestFalse", false);
            using var attributeFalse = file.OpenBooleanAttribute("boolTestFalse");
            Assert.IsFalse(attributeFalse.Read());
            attributeFalse.Write(true);
            Assert.IsTrue(attributeFalse.Read());
        });
    }

    [TestMethod]
    public void CreateReadWriteEnumAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();
            using var size = H5Space.CreateScalar();

            CreateValue(TestInt.Min, TestInt.Max);
            CreateValue(TestUInt.Min, TestUInt.Max);
            CreateValue(TestLong.Min, TestLong.Max);
            CreateValue(TestULong.Min, TestULong.Max);
            CreateValue(TestShort.Min, TestShort.Max);
            CreateValue(TestUShort.Min, TestUShort.Max);
            CreateValue(TestByte.Min, TestByte.Max);
            CreateValue(TestSByte.Min, TestSByte.Max);
            CreateValue(Boolean.False, Boolean.True);

            void CreateValue<T>(T valueMin, T valueMax) where T : unmanaged, Enum
            {
                var nameMin = $"{typeof(T).Name}:{valueMin}";
                file.WriteAttribute<T>(nameMin, valueMin);
                using var attMin = file.OpenEnumAttribute<T>(nameMin);
                Assert.AreEqual(valueMin, attMin.Read());

                var nameMax = $"{typeof(T).Name}:{valueMax}";
                file.WriteAttribute<T>(nameMax, valueMax);
                using var attMax = file.OpenEnumAttribute<T>(nameMax);
                Assert.AreEqual(valueMax, attMax.Read());
            }
        });
    }

    [TestMethod]
    public void CreateReadWriteOverwriteAttributeSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            var status = file.DeleteAttribute("test");
            Assert.AreEqual(DeleteAttributeStatus.NotFound, status);

            file.WriteAttribute("test", 1);
            Assert.AreEqual(1, file.ReadAttribute<int>("test"));

            file.WriteAttribute("test", 2);
            Assert.AreEqual(2, file.ReadAttribute<int>("test"));

            file.WriteAttribute("test", 3.0, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);
            Assert.AreEqual(3.0, file.ReadAttribute<double>("test"));

            status = file.DeleteAttribute("test");
            Assert.AreEqual(DeleteAttributeStatus.Deleted, status);

            // TODO: more testing of AttributeWriteBehaviour and mismatched types
        });
    }

    [TestMethod]
    public void EmptyAttributeNameThrows()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            try
            {
                file.WriteAttribute("", "", 100);
            }
            catch (ArgumentException ex)
            {
                Assert.IsNull(ex.InnerException);

                string msg = ex.Message;
                string toString = ex.ToString();
                Debug.WriteLine(msg);
                Debug.WriteLine(toString);
            }
        });
    }

    [TestMethod]
    public void TooLongAttributeStringValueThrows()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();
            file.WriteAttribute("too_long", "01234567890123456789", 21); // doesn't throw

            using var attribute = file.OpenStringAttribute("too_long");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => attribute.Write("012345678901234567890")); // does throw

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => file.WriteAttribute("too_long2", "012345678901234567890", 21)); // does throw
        });
    }

    [TestMethod]
    public void VariableLengthStringArray()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();
            file.WriteAttribute("strings",
                (IEnumerable<string>)new string[] 
                {
                    "the",
                    "quick",
                    "brown",
                    "fox",
                    "jumped",
                    "over",
                    "the",
                    "lazy",
                    "dog"
                }, 0);
        });
    }

    [TestMethod]
    public void FixedLengthStringArray()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();
            file.WriteAttribute("strings",
                (IEnumerable<string>)new string[] 
                {
                    "the",
                    "quick",
                    "brown",
                    "fox",
                    "jumped",
                    "over",
                    "the",
                    "lazy",
                    "dog"
                }, 10);
        });
    }
}

enum TestInt
{
    None = 0,
    Min = int.MinValue,
    Max = int.MaxValue,
}

enum TestUInt : uint
{
    Min = uint.MinValue,
    Max = uint.MaxValue,
}

enum TestLong : long
{
    None = 0,
    Min = long.MinValue,
    Max = long.MaxValue
}

enum TestULong : ulong
{
    Min = ulong.MinValue,
    Max = ulong.MaxValue
}

enum TestShort : short
{
    None = 0,
    Min = short.MinValue,
    Max = short.MaxValue
}

enum TestUShort : ushort
{
    Min = ushort.MinValue,
    Max = ushort.MaxValue
}

enum TestByte : byte
{
    Min = byte.MinValue,
    Max = byte.MaxValue
}

enum Boolean : byte
{
    False = 0,
    True = 1
}

enum TestSByte : sbyte
{
    None = 0,
    Min = sbyte.MinValue,
    Max = sbyte.MaxValue
}