using System.Diagnostics;

namespace HDF5Api.Tests;

[TestClass]
public class H5GroupTests : H5LocationTests
{
    private const string Path = "test.h5";
    private const string Path1 = "test1.h5";

    [TestMethod]
    public void CreateGroupInGroupSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateGroupSucceeds(group);
        });
    }

    [TestMethod]
    public void CreateAndOpenGroupInGroupSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateAndOpenGroupSucceeds(group);
        });
    }

    [TestMethod]
    public void CreateOpenDeleteGroupInGroupSucceeds()
    {
        HandleCheck(() =>
        {
            // Ensure no existing file
            File.Delete(Path);
            Assert.IsFalse(File.Exists(Path));

            // Create new file
            using var file = H5File.Create(Path);
            Assert.IsTrue(File.Exists(Path));

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateOpenDeleteGroupSucceeds(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            OpenExistingGroupPathSucceeds(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            OpenNonExistingGroupThrows(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateGroupEmptyNameFails(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateDuplicateGroupFails(group);
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

        // Create new group
        using var group = file.CreateGroup("parent");
        Assert.IsTrue(file.GroupExists("parent"));

        GroupExistsReturnsTrueForGroup(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            GroupExistsReturnsFalseForNoGroup(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            GroupExistsThrowsForGroupPathName(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            GroupPathExistsSucceeds(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            GetChildGroupsSucceeds(group);
        });
    }

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

            // Create group
            using var group = file.CreateGroup("group");

            H5AttributeTests.CreateWriteReadDeleteAttributesSucceeds(group);
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

            // Create group
            using var group = file.CreateGroup("group");

            H5AttributeTests.CreateIterateAttributesSucceeds(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateDataSetSucceeds(group);
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

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateAndOpenDataSetSucceeds(group);
        });
    }

    #endregion
}
