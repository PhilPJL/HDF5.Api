﻿namespace HDF5.Api.Tests;

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
    [DataRow("Earliest_18", LibraryVersion.Earliest, LibraryVersion.Version18)]
    [DataRow("Earliest_110", LibraryVersion.Earliest, LibraryVersion.Version110)]
    [DataRow("18_18",LibraryVersion.Version18, LibraryVersion.Version18)]
    [DataRow("18_110", LibraryVersion.Version18, LibraryVersion.Version110)]
    [DataRow("110_110", LibraryVersion.Version110, LibraryVersion.Version110)]
    public void SetLibraryVersionEarliestSucceeds(string name, LibraryVersion low, LibraryVersion high)
    {
        using var acpl = H5File.CreateAccessPropertyList();
        acpl.SetLibraryVersionBounds(new LibraryVersionBounds(low, high));

        HandleCheck2((file) =>
        {
            using var acpl2 = file.GetAccessPropertyList();
            var (low2, high2) = acpl2.GetLibraryVersionBounds();

            Assert.AreEqual(low, low2);
            Assert.AreEqual(high, high2);

            file.SetLibraryVersionBounds(new (LibraryVersion.Version110, LibraryVersion.Version110));
            
            var (low3, high3) = file.GetLibraryVersionBounds();
            Assert.AreEqual(LibraryVersion.Version110, low3);
            Assert.AreEqual(LibraryVersion.Version110, high3);
        },
        name, expectedHandlesOpen: 1, fileAccessPropertyList: acpl);
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
        HandleCheck((file) =>
        {
            CreateGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateAndOpenGroupInFileSucceeds()
    {
        HandleCheck((file) =>
        {
            CreateAndOpenGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateOpenDeleteGroupInFileSucceeds()
    {
        HandleCheck((file) =>
        {
            CreateOpenDeleteGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenExistingGroupPathSucceeds()
    {
        HandleCheck((file) =>
        {
            OpenExistingGroupPathSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenNonExistingGroupThrows()
    {
        HandleCheck((file) =>
        {
            OpenNonExistingGroupThrows(file);
        });
    }

    [TestMethod]
    public void CreateGroupEmptyNameFails()
    {
        HandleCheck((file) =>
        {
            CreateGroupEmptyNameFails(file);
        });
    }

    [TestMethod]
    public void CreateDuplicateGroupFails()
    {
        HandleCheck((file) =>
        {
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
        HandleCheck((file) =>
        {
            GroupExistsReturnsFalseForNoGroup(file);
        });
    }

    [TestMethod]
    public void GroupExistsThrowsForGroupPathName()
    {
        HandleCheck((file) =>
        {
            GroupExistsThrowsForGroupPathName(file);
        });
    }

    [TestMethod]
    public void GroupPathExistsSucceeds()
    {
        HandleCheck((file) =>
        {
            GroupPathExistsSucceeds(file);
        });
    }

    [TestMethod]
    public void GetChildrenSucceeds()
    {
        HandleCheck((file) =>
        {
            GetChildGroupsSucceeds(file);
        });
    }

    [TestMethod]
    public void EnumerateChildrenSucceeds()
    {
        HandleCheck((file) =>
        {
            EnumerateGroupsSucceeds(file);
        });
    }

    #endregion

    #region Attributes

    [TestMethod]
    public void CreateWriteReadDeleteAttributesSucceeds()
    {
        HandleCheck((file) =>
        {
            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateIterateAttributesSucceeds()
    {
        HandleCheck((file) =>
        {
            H5AttributeTests.CreateIterateAttributesSucceeds(file);
        });
    }

    #endregion

    #region Data sets

    [TestMethod]
    public void CreateDataSetSucceeds()
    {
        HandleCheck((file) =>
        {
            CreateDataSetSucceeds(file);

            Assert.AreEqual(1, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void CreateAndOpenDataSetSucceeds()
    {
        HandleCheck((file) =>
        {
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
        HandleCheck2((file) =>
        {
            Assert.AreEqual(GetFileName2(suffix), file.Name);
        },
        suffix);
    }

    [TestMethod]
    public void GetSizeSucceeds()
    {
        HandleCheck((file) =>
        {
            Assert.AreNotEqual(0, file.Size);
        });
    }

    [TestMethod]
    public void FlushSucceeds()
    {
        HandleCheck((file) =>
        {
            file.WriteAttribute("test", "1111111111111111111111111111111111", 50);

            // Flush just for code coverage
            file.Flush();
        });
    }
}
