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
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

            Test(file, "fixed_null", null, 10, characterSet, padding);
            Test(file, "fixed_empty", "", 10, characterSet, padding);
            Test(file, "fixed_22", "12345678912345678912", 32, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "fixed_500", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 500, characterSet, padding);
            }

            static void Test(IH5ObjectWithAttributes file, string name, string? value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
            {
                using var attribute = file.CreateStringAttribute(name, fixedStorageLength, characterSet, padding);

                if (value != null)
                {
                    attribute.Write(value);
                }

                var r = attribute.ReadString();
                
                // NOTE: assuming that if no value is written ReadString will return string.Empty
                Assert.AreEqual(value ?? string.Empty, r);

                Assert.AreEqual(name, attribute.Name);
            }
        });
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
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileNameSuffix);

            Test(file, "variable_null", null, 0, characterSet, padding);
            Test(file, "variable_empty", "", 0, characterSet, padding);
            Test(file, "variable_short", "12345678912345678912", 0, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "variable_long", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 0, characterSet, padding);
            }

            static void Test(IH5ObjectWithAttributes file, string name, string? value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
            {
                using (var attribute = file.CreateStringAttribute(name, fixedStorageLength, characterSet, padding))
                {
                    if (value != null)
                    {
                        attribute.Write(value);
                    }
                }

                using (var attribute = file.OpenAttribute(name))
                {
                    var r = attribute.ReadString();

                    // NOTE: assuming that if no value is written ReadString will return string.Empty
                    Assert.AreEqual(value ?? string.Empty, r);

                    Assert.AreEqual(name, attribute.Name);
                }
            }
        });
    }

    [TestMethod]
    [DataRow("ascii", "an ascii attribute name")]
    [DataRow("utf8", "Χαρακτηριστικό")]
    public void CreateDuplicateAttributeNameThrows(string filename, string attributeName)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(filename);

            // Create group
            using var group = file.CreateGroup("group");

            // Create duplicate attribute on file
            file.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => file.CreateAndWriteAttribute(attributeName, 2));

            // Create duplicate attribute on group
            group.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => group.CreateAndWriteAttribute(attributeName, 2));

            // Create test dataset on file
            using var datasetFile = H5DataSetTests.CreateTestDataset(file, "aDataSet");

            // Create duplicate attribute on data set on file
            datasetFile.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => datasetFile.CreateAndWriteAttribute(attributeName, 2));

            // Create test dataset on group
            using var datasetGroup = H5DataSetTests.CreateTestDataset(group, "aDataSet");

            // Create duplicate attribute on data set on group
            datasetGroup.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<H5Exception>(() => datasetGroup.CreateAndWriteAttribute(attributeName, 2));

            // File + Group + 2 x DataSet
            Assert.AreEqual(4, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void RewriteStringAttributeSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            // Create group
            using var group = file.CreateGroup("group");

            group.CreateAndWriteAttribute("int", 1);
            group.CreateAndWriteAttribute("string", "short", 10);
            group.CreateAndWriteAttribute("long", 1L);

            group.DeleteAttribute("string");

            const string s = "long-----------------------------------";
            group.CreateAndWriteAttribute("string", s, 100);

            string s1 = group.ReadStringAttribute("string");
            Assert.AreEqual(s, s1);

            // File + Group
            Assert.AreEqual(2, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void ReadInvalidAttributeTypeFails()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            // Create group
            using var group = file.CreateGroup("group");

            group.CreateAndWriteAttribute("int", 1);
            group.CreateAndWriteAttribute("string", "short", 10);

            string s1 = group.ReadStringAttribute("string");
            Assert.AreEqual("short", s1);

            int i1 = group.ReadAttribute<int>("int");
            Assert.AreEqual(1, i1);

            Assert.ThrowsException<H5Exception>(() => group.ReadStringAttribute("int"));
            Assert.ThrowsException<H5Exception>(() => group.ReadAttribute<int>("string"));
        });
    }

    // Helper methods
    internal static void CreateIterateAttributesSucceeds(IH5ObjectWithAttributes objectWithAttributes)
    {
        objectWithAttributes.CreateAndWriteAttribute("string", "This is a string 12345.", 50);
        objectWithAttributes.CreateAndWriteAttribute("dateTime", DateTime.Now);
        objectWithAttributes.CreateAndWriteAttribute("bool-true", true);
        objectWithAttributes.CreateAndWriteAttribute("bool-false", false);

        objectWithAttributes.CreateAndWriteAttribute("byte-min", byte.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("byte", (byte)5);
        objectWithAttributes.CreateAndWriteAttribute("byte-max", byte.MaxValue);
        
        objectWithAttributes.CreateAndWriteAttribute("int16-min", short.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("int16", (short)5);
        objectWithAttributes.CreateAndWriteAttribute("int16-max", short.MaxValue);

        objectWithAttributes.CreateAndWriteAttribute("uint16-min", ushort.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("uint16", (ushort)6);
        objectWithAttributes.CreateAndWriteAttribute("uint16-max", ushort.MaxValue);

        objectWithAttributes.CreateAndWriteAttribute("int32-min", int.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("int32", 7);
        objectWithAttributes.CreateAndWriteAttribute("int32-max", int.MaxValue);

        objectWithAttributes.CreateAndWriteAttribute("uint32-min", uint.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("uint32", 8u);
        objectWithAttributes.CreateAndWriteAttribute("uint32-max", uint.MaxValue);

        objectWithAttributes.CreateAndWriteAttribute("long-min", long.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("long", 9L);
        objectWithAttributes.CreateAndWriteAttribute("long-max", long.MaxValue);
        
        objectWithAttributes.CreateAndWriteAttribute("ulong-min", ulong.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("ulong", 10ul);
        objectWithAttributes.CreateAndWriteAttribute("ulong-max", ulong.MaxValue);
        
        objectWithAttributes.CreateAndWriteAttribute("float-min", float.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("float", 11.0f);
        objectWithAttributes.CreateAndWriteAttribute("float-max", float.MaxValue);
        
        objectWithAttributes.CreateAndWriteAttribute("double-min", double.MinValue);
        objectWithAttributes.CreateAndWriteAttribute("double", 12.0d);
        objectWithAttributes.CreateAndWriteAttribute("double-max", double.MaxValue);

        // TODO: decimal

        var names = new List<string> {
            "dateTime",
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
            "double-max"
        }
        .OrderBy(a => a)
        .ToList();

        var attributeNames = objectWithAttributes.AttributeNames.OrderBy(a => a).ToList();

        Assert.IsTrue(names.SequenceEqual(attributeNames));
    }

    internal static void CreateWriteReadDeleteAttributesSucceeds(IH5ObjectWithAttributes location)
    {
        // Create attributes
        CreateWriteReadUpdateDeleteAttribute((short)15, (short)99);
        CreateWriteReadUpdateDeleteAttribute((ushort)15, (ushort)77);
        CreateWriteReadUpdateDeleteAttribute((byte)15, (byte)0xff);
        //CreateWriteReadUpdateDeleteBoolAttribute(true, false);
        CreateWriteReadUpdateDeleteAttribute(15, 1234);
        CreateWriteReadUpdateDeleteAttribute(15u, 4567u);
        CreateWriteReadUpdateDeleteAttribute(15L, long.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(15uL, ulong.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(1.5f, 123.456);
        CreateWriteReadUpdateDeleteAttribute(1.5123d, double.MaxValue);
        CreateWriteReadUpdateDeleteStringAttribute("1234567890", "ABCDEFGH", 11);
        CreateWriteReadUpdateDeleteStringAttribute("", "ABCDEFGH", 10);
        CreateWriteReadUpdateDeleteStringAttribute("abcdefghijklmnopqrstuvwzyz", "", 27);
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddYears(5));
        CreateWriteReadUpdateDeleteStringAttribute(new string('A', 1000), new string('B', 500), 1200);
        Assert.AreEqual(0, location.NumberOfAttributes);

        void CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime value, DateTime newValue)
        {
            const string name = "dtDateTime";

            location.CreateAndWriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            var readValue = location.ReadDateTimeAttribute(name);
            // There is some loss of precision since we're storing DateTime as a double
            Assert.IsTrue(Math.Abs((value - readValue).TotalMilliseconds) < 1);

            using var a = location.OpenAttribute(name);
            a.Write(newValue);

            var readValue2 = location.ReadDateTimeAttribute(name);
            Assert.IsTrue(Math.Abs((newValue - readValue2).TotalMilliseconds) < 1);

            var readValue3 = a.ReadDateTime();
            Assert.IsTrue(Math.Abs((newValue - readValue2).TotalMilliseconds) < 1);

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteStringAttribute(string value, string newValue, int fixedStorageLength = 0)
        {
            const string name = "dtString";

            location.CreateAndWriteAttribute(name, value, fixedStorageLength);
            Assert.IsTrue(location.AttributeExists(name));

            string readValue = location.ReadStringAttribute(name);

            Assert.AreEqual(value, readValue);

            using var a = location.OpenAttribute(name);
            string readValue2 = location.ReadStringAttribute(name);

            Assert.AreEqual(value, readValue2);

            a.Write(newValue);

            string readValue3 = location.ReadStringAttribute(name);
            Assert.AreEqual(value, readValue);

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteAttribute<T>(T value, T newValue) where T : unmanaged, IEquatable<T>
        {
            string name = $"dt{typeof(T).Name}";

            location.CreateAndWriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            var readValue = location.ReadAttribute<T>(name);
            Assert.AreEqual(value, readValue);

            using var a = location.OpenAttribute(name);
            var readValue2 = location.ReadAttribute<T>(name);
            Assert.AreEqual(value, readValue2);

            a.Write(newValue);
            var readValue3 = location.ReadAttribute<T>(name);
            Assert.AreEqual(newValue, readValue3);

            location.DeleteAttribute(name);
            Assert.IsFalse(location.AttributeExists(name));
        }

        void CreateWriteReadUpdateDeleteBoolAttribute(bool value, bool newValue)
        {
            string name = $"dt{typeof(bool).Name}";

            location.CreateAndWriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            var readValue = location.ReadBoolAttribute(name);
            Assert.AreEqual(value, readValue);

            using var a = location.OpenAttribute(name);
            var readValue2 = location.ReadBoolAttribute(name);
            Assert.AreEqual(value, readValue2);

            a.Write(newValue);
            var readValue3 = location.ReadBoolAttribute(name);
            Assert.AreEqual(newValue, readValue3);

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
            using var attribute = file.CreateStringAttribute(attributeName, value.Length * 4, encoding);

            attribute.Write(value);
            var read = attribute.ReadString();
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
            using var attribute = file.CreateStringAttribute("duplicate", 100);

            try
            {
                using var attribute2 = file.CreateStringAttribute("duplicate", 100);
            }
            catch (H5Exception ex)
            {
                Assert.IsNull(ex.InnerException);

                string msg = ex.Message;
                string toString = ex.ToString();
                Debug.WriteLine(msg);
                Debug.WriteLine(toString);

                Assert.AreEqual("attribute already exists→unable to create attribute", msg);
            }
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
                using var attribute = file.CreateStringAttribute("", 100);
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
            using var attribute = file.CreateStringAttribute("too_long", 21);

            attribute.Write("01234567890123456789"); // doesn't throw

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => attribute.Write("012345678901234567890")); // does throw
        });
    }
}
