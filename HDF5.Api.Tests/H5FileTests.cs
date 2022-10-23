namespace HDF5.Api.Tests;

[TestClass]
public class H5FileTests : H5LocationTests<H5FileTests>
{
    #region Create file tests

    [TestMethod]
    public void CreateFileDanglingHandle()
    {
        H5File? file = null;

        HandleCheck(() =>
        {
            file = CreateFile();
        }, 1);

        file?.Dispose();
    }

    [TestMethod]
    public void CreateFileOverwriteExistingSucceeds()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateFile()) { }

            // Create new file - overwrite existing
            using (CreateFile()) { }
        });
    }

    [TestMethod]
    public void CreateFileThrowsIfAlreadyExists()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateFile()) { }

            // Create and throw if exists
            Assert.ThrowsException<H5Exception>(() => CreateFile(failIfExists: true));
        });
    }

    [TestMethod]
    public void CreateFileOverwriteOpenFileThrows()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateFile()) // leave file open - don't dispose until after second open attempt
            {
                // Try to create new file and overwrite existing
                Assert.ThrowsException<H5Exception>(() => CreateFile());
            }
        });
    }

    [TestMethod]
    public void CreateOrOpenNewFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateOrOpenFile()) { }
        });
    }

    [TestMethod]
    public void CreateOrOpenExistingFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateOrOpenFile()) { }

            // Create new file
            using (CreateOrOpenFile()) { }
        });
    }

    [TestMethod]
    [DataRow(LibraryVersion.Earliest, LibraryVersion.Version18)]
    [DataRow(LibraryVersion.Earliest, LibraryVersion.Version110)]
    [DataRow(LibraryVersion.Version18, LibraryVersion.Version18)]
    [DataRow(LibraryVersion.Version18, LibraryVersion.Version110)]
    [DataRow(LibraryVersion.Version110, LibraryVersion.Version110)]
    public void SetLibraryVersionEarliestSucceeds(LibraryVersion low, LibraryVersion high)
    {
        HandleCheck(() =>
        {
            using var acpl = H5File.CreateAccessPropertyList();
            acpl.SetLibraryVersionBounds(low, high);

            // Create new file
            using var file = CreateOrOpenFile(fileAccessPropertyList: acpl);

            using var acpl2 = file.GetAccessPropertyList();
            var (low2, high2) = acpl2.GetLibraryVersionBounds();

            Assert.AreEqual(low, low2);
            Assert.AreEqual(high, high2);

            file.SetLibraryVersionBounds(LibraryVersion.Version110, LibraryVersion.Version110);
            
            var (low3, high3) = file.GetLibraryVersionBounds();
            Assert.AreEqual(LibraryVersion.Version110, low3);
            Assert.AreEqual(LibraryVersion.Version110, high3);
        });
    }

    #endregion

    #region Open file tests

    [TestMethod]
    public void OpenFileThatDoesNotExistThrows()
    {
        HandleCheck(() =>
        {
            const string path = "none.h5";

            // Ensure no existing file
            File.Delete(path);
            Assert.IsFalse(File.Exists(path));

            // Try to open missing existing file read-only
            Assert.ThrowsException<H5Exception>(() => H5File.Open(path, true));
        });
    }

    [TestMethod]
    public void OpenFileThatDoesExistSucceeds()
    {
        HandleCheck(() =>
        {
            using (CreateFile()) { }

            using var file = CreateFile();
        });
    }

    [TestMethod]
    public void OpenFileThatDoesExistSucceedsAndIsReadOnly()
    {
        HandleCheck(() =>
        {
            // Create new file
            using (CreateFile()) { }

            // Try to open missing existing file read-only
            using var file = OpenFile(readOnly: true);

            // Try to write to file should fail (assumes CreateGroup is valid)
            Assert.ThrowsException<H5Exception>(() => file.CreateGroup("test"));
        });
    }

    #endregion

    #region Groups

    [TestMethod]
    public void CreateGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateAndOpenGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateAndOpenGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateOpenDeleteGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateOpenDeleteGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenExistingGroupPathSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            OpenExistingGroupPathSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenNonExistingGroupThrows()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            OpenNonExistingGroupThrows(file);
        });
    }

    [TestMethod]
    public void CreateGroupEmptyNameFails()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateGroupEmptyNameFails(file);
        });
    }

    [TestMethod]
    public void CreateDuplicateGroupFails()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateDuplicateGroupFails(file);
        });
    }

    [TestMethod]
    public void GroupExistsReturnsTrueForGroup()
    {
        using var file = CreateFile();

        GroupExistsReturnsTrueForGroup(file);
    }

    [TestMethod]
    public void GroupExistsReturnsFalseForNoGroup()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            GroupExistsReturnsFalseForNoGroup(file);
        });
    }

    [TestMethod]
    public void GroupExistsThrowsForGroupPathName()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            GroupExistsThrowsForGroupPathName(file);
        });
    }

    [TestMethod]
    public void GroupPathExistsSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            GroupPathExistsSucceeds(file);
        });
    }

    [TestMethod]
    public void GetChildrenSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            GetChildGroupsSucceeds(file);
        });
    }

    [TestMethod]
    public void EnumerateChildrenSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            EnumerateGroupsSucceeds(file);
        });
    }

    #endregion

    #region Attributes

    [TestMethod]
    public void CreateWriteReadDeleteAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateIterateAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            H5AttributeTests.CreateIterateAttributesSucceeds(file);
        });
    }

    #endregion

    #region Data sets

    [TestMethod]
    public void CreateDataSetSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateDataSetSucceeds(file);

            Assert.AreEqual(1, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void CreateAndOpenDataSetSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            CreateAndOpenDataSetSucceeds(file);

            Assert.AreEqual(1, file.GetObjectCount());
        });
    }

    #endregion

    [TestMethod]
    [DataRow("Ascii")]
    [DataRow("ᚪ᛫ᚷᛖᚻᚹᛦᛚ")]
    [DataRow("Χαρακτηριστικό")]
    public void GetNameSucceeds(string suffix)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(suffix);

            Assert.AreEqual(GetFileName2(suffix), file.Name);
        });
    }

    [TestMethod]
    public void GetSizeSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            Assert.AreNotEqual(0, file.Size);
        });
    }

    [TestMethod]
    public void FlushSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            file.CreateAndWriteAttribute("test", "1111111111111111111111111111111111", 50);

            // Flush just for code coverage
            file.Flush();
        });
    }
}
