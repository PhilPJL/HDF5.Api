using System.Diagnostics;

namespace HDF5.Api.Tests;

[TestClass]
public class H5AttributeTests : H5Test
{
    private const string Path = "test.h5";

/*    [TestMethod]
    public void Test()
    {
        HandleCheck(() =>
        {
            using var file = H5File.Open(@"D:\TeraViewTestFiles\C123456CE123.tprj");
            using var grp = file.OpenGroup("TerapulseDocument");
            using var att = grp.OpenAttribute("ClassName");
            Debug.WriteLine(att.ReadString());
        });
    }
*/
    [TestMethod]
    public void CreateDuplicateAttributeNameThrows()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create group
            using var group = file.CreateGroup("group");

            // Create duplicate attribute on file
            file.CreateAndWriteAttribute("name", 1);
            Assert.ThrowsException<Hdf5Exception>(() => file.CreateAndWriteAttribute("name", 2));

            // Create duplicate attribute on group
            group.CreateAndWriteAttribute("name", 1);
            Assert.ThrowsException<Hdf5Exception>(() => group.CreateAndWriteAttribute("name", 2));

            // Create test dataset on file
            using var datasetFile = H5DataSetTests.CreateTestDataset(file, "aDataSet");

            // Create duplicate attribute on data set on file
            datasetFile.CreateAndWriteAttribute("name", 1);
            Assert.ThrowsException<Hdf5Exception>(() => datasetFile.CreateAndWriteAttribute("name", 2));

            // Create test dataset on group
            using var datasetGroup = H5DataSetTests.CreateTestDataset(group, "aDataSet");

            // Create duplicate attribute on data set on group
            datasetGroup.CreateAndWriteAttribute("name", 1);
            Assert.ThrowsException<Hdf5Exception>(() => datasetGroup.CreateAndWriteAttribute("name", 2));

            // File + Group + 2 x DataSet
            Assert.AreEqual(4, file.GetObjectCount());
        });
    }


    [TestMethod]
    public void CreateDuplicateAttributeNameThrows2()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create group
            using var group = file.CreateGroup("group");

            // Create duplicate attribute on file
            file.CreateAndWriteAttribute("name", 1);
            try
            {
                file.CreateAndWriteAttribute("name", 2);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            };
        });
    }

    [TestMethod]
    public void RewriteStringAttributeSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create group
            using var group = file.CreateGroup("group");

            group.CreateAndWriteAttribute("int", 1);
            group.CreateAndWriteAttribute("string", "short");
            group.CreateAndWriteAttribute("long", 1L);

            group.DeleteAttribute("string");

            const string s = "long-----------------------------------";
            group.CreateAndWriteAttribute("string", s);

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
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create group
            using var group = file.CreateGroup("group");

            group.CreateAndWriteAttribute("int", 1);
            group.CreateAndWriteAttribute("string", "short");

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
        objectWithAttributes.CreateAndWriteAttribute("string", "This is a string 12345.");
        objectWithAttributes.CreateAndWriteAttribute("truncated", "This is a string 12345.", 10);
        objectWithAttributes.CreateAndWriteAttribute("dateTime", DateTime.Now);
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
        Assert.AreEqual(11, objectWithAttributes.NumberOfAttributes);
    }

    internal static void CreateWriteReadDeleteAttributesSucceeds(IH5ObjectWithAttributes location)
    {
        // Create attributes
        CreateWriteReadUpdateDeleteAttribute((short)15, (short)99);
        CreateWriteReadUpdateDeleteAttribute((ushort)15, (ushort)77);
        CreateWriteReadUpdateDeleteAttribute(15, 1234);
        CreateWriteReadUpdateDeleteAttribute(15u, 4567u);
        CreateWriteReadUpdateDeleteAttribute(15L, long.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(15uL, ulong.MaxValue);
        CreateWriteReadUpdateDeleteAttribute(1.5f, 123.456);
        CreateWriteReadUpdateDeleteAttribute(1.5123d, double.MaxValue);
        CreateWriteReadUpdateDeleteStringAttribute("1234567890", "ABCDEFGH");
        CreateWriteReadUpdateDeleteStringAttribute("1234567890", "ABCDEFGH", 5);
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
        CreateWriteReadUpdateDeleteDateTimeAttribute(DateTime.UtcNow, DateTime.UtcNow.AddYears(5));
        CreateWriteReadUpdateDeleteStringAttribute(new string('A', 1000), new string('B', 500));
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

        void CreateWriteReadUpdateDeleteStringAttribute(string value, string newValue, int maxLength = 0)
        {
            const string name = "dtString";

            location.CreateAndWriteAttribute(name, value, maxLength);
            Assert.IsTrue(location.AttributeExists(name));

            string readValue = location.ReadStringAttribute(name);

            if (maxLength != 0)
            {
                Assert.AreEqual(maxLength, readValue.Length);
                Assert.IsTrue(value.StartsWith(readValue, StringComparison.Ordinal));
            }
            else
            {
                Assert.AreEqual(value, readValue);
            }

            using var a = location.OpenAttribute(name);
            string readValue2 = location.ReadStringAttribute(name);

            if (maxLength != 0)
            {
                Assert.AreEqual(maxLength, readValue2.Length);
                Assert.IsTrue(value.StartsWith(readValue, StringComparison.Ordinal));
            }
            else
            {
                Assert.AreEqual(value, readValue2);
            }

            a.Write(newValue);

            string readValue3 = location.ReadStringAttribute(name);
            if (maxLength != 0)
            {
                Assert.AreEqual(maxLength, readValue3.Length);
                Assert.IsTrue(newValue.StartsWith(readValue3, StringComparison.Ordinal));
            }
            else
            {
                Assert.AreEqual(value, readValue);
            }

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
}
