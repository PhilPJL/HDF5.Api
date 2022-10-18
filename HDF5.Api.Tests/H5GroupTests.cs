﻿using System.Diagnostics;

namespace HDF5.Api.Tests;

[TestClass]
public class H5GroupTests : H5LocationTests<H5GroupTests>
{
    [TestMethod]
    public void CreateGroupInGroupSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateDuplicateGroupFails(group);
        });
    }

    [TestMethod]
    public void GroupExistsReturnsTrueForGroup()
    {
        using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

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
            using var file = CreateFile();

            // Create new group
            using var group = file.CreateGroup("parent");
            Assert.IsTrue(file.GroupExists("parent"));

            CreateAndOpenDataSetSucceeds(group);
        });
    }

    #endregion

    [TestMethod]
    [DataRow("ansi", "group")]
    [DataRow("utf8", "Χαρακτηριστικό")]
    public void CreateGroupAndCheckNameSucceeds(string suffix, string name)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(suffix);
            using var group = file.CreateGroup(name);

            Assert.IsTrue(file.GroupNames.Contains(name));
            Assert.IsTrue(file.GroupExists(name));

            // group.Name returns rooted path name
            Assert.AreEqual("/" + name, group.Name);
        });
    }

    [TestMethod]
    [DataRow("ansi", "group")]
    [DataRow("utf8", "Χαρακτηριστικό")]
    public void CreateGroupAndCheckNameSucceeds2(string suffix, string name)
    {
        HandleCheck(() =>
        {
            using var file = CreateFile2(suffix);
            using var parent = file.CreateGroup("parent");
            using var group = parent.CreateGroup(name);

            Assert.IsTrue(parent.GroupNames.Contains(name));
            Assert.IsTrue(parent.GroupExists(name));

            // group.Name returns rooted path name
            Assert.AreEqual("/parent/" + name, group.Name);
        });
    }
}
