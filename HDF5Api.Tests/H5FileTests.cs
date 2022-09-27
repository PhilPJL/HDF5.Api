namespace HDF5Api.Tests;

[TestClass]
public class H5FileTests : H5LocationTests
{
    private const string Path = "test.h5";

    #region Create file tests

    [TestMethod]
    public void OpenHandleTest()
    {
        H5File file = null;

        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            file = H5File.Create(Path);

        }, 1);

        file?.Dispose();
    }

    [TestMethod]
    public void CreateFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using (H5File.Create(Path)) { }

            Assert.IsTrue(File.Exists(Path));
        });
    }

    [TestMethod]
    public void CreateFileOverwriteExistingSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using (H5File.Create(Path)) { }

            Assert.IsTrue(File.Exists(Path));

            // Create new file - overwrite existing
            using (H5File.Create(Path)) { }

            Assert.IsTrue(File.Exists(Path));
        });
    }

    [TestMethod]
    public void CreateFileOverwriteExistingThrows()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using (H5File.Create(Path)) // leave file open - don't dispose until after second open attempt
            {
                Assert.IsTrue(File.Exists(Path));

                // Try to create new file and overwrite existing
                Assert.ThrowsException<Hdf5Exception>(() => H5File.Create(Path));
            }
        });
    }

    #endregion

    #region Open file tests

    [TestMethod]
    public void OpenFileThatDoesNotExistThrows()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Try to open missing existing file read-only
            Assert.ThrowsException<Hdf5Exception>(() => H5File.Open(Path, true));
        });
    }

    [TestMethod]
    public void OpenFileThatDoesExistSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using (H5File.Create(Path)) { }

            Assert.IsTrue(File.Exists(Path));

            // Try to open missing existing file read-only
            using var file = H5File.Open(Path, true);
        });
    }

    [TestMethod]
    public void OpenFileThatDoesExistSucceedsAndIsReadOnly()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using (H5File.Create(Path)) { }

            Assert.IsTrue(File.Exists(Path));

            // Try to open missing existing file read-only
            using var file = H5File.Open(Path, true);

            // Try to write to file should fail (assumes CreateGroup is valid)
            Assert.ThrowsException<Hdf5Exception>(() => file.CreateGroup("test"));
        });
    }

    #endregion

    #region Groups

    [TestMethod]
    public void CreateGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateAndOpenGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateAndOpenGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateOpenDeleteGroupInFileSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateOpenDeleteGroupSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenExistingGroupPathSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            OpenExistingGroupPathSucceeds(file);
        });
    }

    [TestMethod]
    public void OpenNonExistingGroupThrows()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            OpenNonExistingGroupThrows(file);
        });
    }

    [TestMethod]
    public void CreateGroupEmptyNameFails()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateGroupEmptyNameFails(file);
        });
    }

    [TestMethod]
    public void CreateDuplicateGroupFails()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateDuplicateGroupFails(file);
        });
    }

    [TestMethod]
    public void GroupExistsReturnsTrueForGroup()
    {
        // Ensure no existing file
        File.Delete(Path);
        Assert.IsFalse(File.Exists(Path));

        // Create new file
        using var file = H5File.Create(Path);
        Assert.IsTrue(File.Exists(Path));

        GroupExistsReturnsTrueForGroup(file);
    }

    [TestMethod]
    public void GroupExistsReturnsFalseForNoGroup()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            GroupExistsReturnsFalseForNoGroup(file);
        });
    }

    [TestMethod]
    public void GroupExistsThrowsForGroupPathName()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            GroupExistsThrowsForGroupPathName(file);
        });
    }

    [TestMethod]
    public void GroupPathExistsSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            GroupPathExistsSucceeds(file);
        });
    }

    [TestMethod]
    public void GetChildrenSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            GetChildGroupsSucceeds(file);
        });
    }

    #endregion

    #region Attributes

    [TestMethod]
    public void CreateWriteReadDeleteAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(file);
        });
    }

    [TestMethod]
    public void CreateIterateAttributesSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

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
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateDataSetSucceeds(file);

            Assert.AreEqual(1, file.GetObjectCount());
        });
    }

    [TestMethod]
    public void CreateAndOpenDataSetSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            CreateAndOpenDataSetSucceeds(file);

            Assert.AreEqual(1, file.GetObjectCount());
        });
    }

    #endregion
}
