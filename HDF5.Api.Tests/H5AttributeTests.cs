using System.Diagnostics;
using System.Text;

namespace HDF5.Api.Tests;

[TestClass]
public class H5AttributeTests : H5Test
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

            Test(file, "fixed_10", "", 10, characterSet, padding);
            Test(file, "fixed_32", "12345678912345678912", 32, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters long but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "fixed_500", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 500, characterSet, padding);
            }

            static void Test(H5File file, string name, string value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
            {
                using var attribute = file.CreateStringAttribute(name, fixedStorageLength, characterSet, padding);
                attribute.Write(value);
                var r = attribute.ReadString();
                Assert.AreEqual(value, r);
            }
        });
    }

    [Ignore]
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

            Test(file, "variable_empty", "", 0, characterSet, padding);
            Test(file, "variable_short", "12345678912345678912", 0, characterSet, padding);

            if (characterSet == CharacterSet.Utf8)
            {
                // NOTE that this Unicode string is 107 characters long but requires 321 bytes of storage.
                // HDF5 fixed length storage indicates the number of bytes not the number of characters.
                Test(file, "variable_long", "ᚠᛇᚻ᛫ᛒᛦᚦ᛫ᚠᚱᚩᚠᚢᚱ᛫ᚠᛁᚱᚪ᛫ᚷᛖᚻᚹᛦᛚᚳᚢᛗᛋᚳᛖᚪᛚ᛫ᚦᛖᚪᚻ᛫ᛗᚪᚾᚾᚪ᛫ᚷᛖᚻᚹᛦᛚᚳ᛫ᛗᛁᚳᛚᚢᚾ᛫ᚻᛦᛏ᛫ᛞᚫᛚᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻᛏᚾᛖ᛫ᛞᚩᛗᛖᛋ᛫ᚻᛚᛇᛏᚪᚾ᛬", 0, characterSet, padding);
            }

            static void Test(H5File file, string name, string value, int fixedStorageLength, CharacterSet characterSet, StringPadding padding)
            {
                using var attribute = file.CreateStringAttribute(name, fixedStorageLength, characterSet, padding);
                attribute.Write(value);
                var r = attribute.ReadString();
                Assert.AreEqual(value, r);
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
            Assert.ThrowsException<Hdf5Exception>(() => file.CreateAndWriteAttribute(attributeName, 2));

            // Create duplicate attribute on group
            group.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<Hdf5Exception>(() => group.CreateAndWriteAttribute(attributeName, 2));

            // Create test dataset on file
            using var datasetFile = H5DataSetTests.CreateTestDataset(file, "aDataSet");

            // Create duplicate attribute on data set on file
            datasetFile.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<Hdf5Exception>(() => datasetFile.CreateAndWriteAttribute(attributeName, 2));

            // Create test dataset on group
            using var datasetGroup = H5DataSetTests.CreateTestDataset(group, "aDataSet");

            // Create duplicate attribute on data set on group
            datasetGroup.CreateAndWriteAttribute(attributeName, 1);
            Assert.ThrowsException<Hdf5Exception>(() => datasetGroup.CreateAndWriteAttribute(attributeName, 2));

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

            Assert.ThrowsException<Hdf5Exception>(() => group.ReadStringAttribute("int"));
            Assert.ThrowsException<Hdf5Exception>(() => group.ReadAttribute<int>("string"));
        });
    }

    // Helper methods
    internal static void CreateIterateAttributesSucceeds(IH5ObjectWithAttributes objectWithAttributes)
    {
        objectWithAttributes.CreateAndWriteAttribute("string", "This is a string 12345.", 50);
        objectWithAttributes.CreateAndWriteAttribute("truncated", "This is a string 12345.", 50);
        objectWithAttributes.CreateAndWriteAttribute("dateTime", DateTime.Now);
        //objectWithAttributes.CreateAndWriteAttribute("bool", true);
        objectWithAttributes.CreateAndWriteAttribute("byte", (byte)5);
        objectWithAttributes.CreateAndWriteAttribute("int16", (short)5);
        objectWithAttributes.CreateAndWriteAttribute("uint16", (ushort)6);
        objectWithAttributes.CreateAndWriteAttribute("int32", 7);
        objectWithAttributes.CreateAndWriteAttribute("uint32", 8u);
        objectWithAttributes.CreateAndWriteAttribute("long", 9L);
        objectWithAttributes.CreateAndWriteAttribute("ulong", 10ul);
        objectWithAttributes.CreateAndWriteAttribute("float", 11.0f);
        objectWithAttributes.CreateAndWriteAttribute("double", 12.0d);

        var names = new List<string> {
            "dateTime",
            //"bool",
            "byte",
            "string",
            "truncated",
            "int16",
            "uint16",
            "int32",
            "uint32",
            "long",
            "ulong",
            "float",
            "double"
        };

        var attributeNames = objectWithAttributes.AttributeNames;

        Assert.IsTrue(!names.Except(attributeNames).Any());
        Assert.IsTrue(!attributeNames.Except(names).Any());
        Assert.AreEqual(names.Count, objectWithAttributes.NumberOfAttributes);
    }

    internal static void CreateWriteReadDeleteAttributesSucceeds(IH5ObjectWithAttributes location)
    {
        // Create attributes
        CreateWriteReadUpdateDeleteAttribute((short)15, (short)99);
        CreateWriteReadUpdateDeleteAttribute((ushort)15, (ushort)77);
        CreateWriteReadUpdateDeleteAttribute((byte)15, (byte)0xff);
        //CreateWriteReadUpdateDeleteAttribute(true, false);
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

        void CreateWriteReadUpdateDeleteAttribute<T>(T value, T newValue) where T : unmanaged
        {
            string name = $"dt{typeof(T).Name}";

            location.CreateAndWriteAttribute(name, value);
            Assert.IsTrue(location.AttributeExists(name));

            T readValue = location.ReadAttribute<T>(name);
            Assert.AreEqual(value, readValue);

            using var a = location.OpenAttribute(name);
            T readValue2 = location.ReadAttribute<T>(name);
            Assert.AreEqual(value, readValue2);

            a.Write(newValue);
            T readValue3 = location.ReadAttribute<T>(name);
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
    public void CreateAttributeSucceeds(string fileSuffix, string attributeName, string value, CharacterSet encoding)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(fileSuffix);
            using var attribute = file.CreateStringAttribute(attributeName, 100, encoding);

            attribute.Write(value);
            var read = attribute.ReadString();
            Assert.AreEqual(value, read);

            Assert.IsTrue(file.AttributeExists(attributeName));
            file.AttributeNames.ForEach(a => Debug.WriteLine(a));
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
            catch (Hdf5Exception ex)
            {
                Assert.IsNull(ex.InnerException);

                string msg = ex.Message;
                string toString = ex.ToString();
                Debug.WriteLine(msg);
                Debug.WriteLine(toString);

                Assert.AreEqual("0:unable to create attribute/1:attribute already exists", msg);
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
}
