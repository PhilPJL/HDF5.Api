using HDF5.Api.NativeMethodAdapters;
using System.Diagnostics;

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

    [TestMethod]
    public void EnumerateChildrenSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            EnumerateGroupsSucceeds(file);
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

    [TestMethod]
    public void CreateIntermediateGroupsSucceeds()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            const string groupPath = "parent/child/grandchild";

            using var grandChild = file.CreateGroup(groupPath);

            Assert.AreEqual("/" + groupPath, grandChild.Name);

            Assert.IsTrue(file.GroupPathExists(groupPath));
            Assert.IsTrue(file.GroupPathExists(groupPath));
            Assert.IsTrue(file.GroupNames.Contains("parent"));

            using var parent = file.OpenGroup("parent");
            Assert.IsTrue(parent.GroupNames.Contains("child"));
            Assert.IsTrue(parent.GroupExists("child"));
            Assert.IsTrue(parent.GroupPathExists("child/grandchild"));

            using var child = parent.OpenGroup("child");
            Assert.IsTrue(child.GroupNames.Contains("grandchild"));
            Assert.IsTrue(child.GroupExists("grandchild"));
            Assert.IsTrue(child.GroupPathExists("grandchild"));
        });
    }

    [TestMethod]
    public void CreateIntermediateGroupsThrows()
    {
        HandleCheck(() =>
        {
            using var file = CreateFile();

            const string groupPath = "parent/child/grandchild";

            using var lcpl = H5Link.CreateCreationPropertyList(createIntermediateGroups: false);

            Assert.ThrowsException<H5Exception>(() =>
            {
                try
                {
                    H5GAdapter.Create(file, groupPath, lcpl, null);
                }
                catch(H5Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            });
        });
    }
}
