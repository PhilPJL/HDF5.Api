﻿using HDF.PInvoke;

namespace HDF5Api.Tests;

public abstract class H5LocationTests : H5Test
{
    protected H5LocationTests()
    {
    }

    #region Groups
    protected static void CreateGroupSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        const string grp1Name = "grp1";
        const string grp2Name = "grp2";

        using var grp1 = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        using var grp2 = grp1.CreateGroup(grp2Name);
        Assert.IsTrue(grp1.GroupExists(grp2Name));

        Assert.IsTrue(location.GroupPathExists($"{grp1Name}/{grp2Name}"));
    }

    protected static void CreateAndOpenGroupSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Try to open
        using var group2 = location.OpenGroup(grp1Name);
    }

    protected static void CreateOpenDeleteGroupSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Try to open
        using var group2 = location.OpenGroup(grp1Name);

        // Delete when group is open
        location.DeleteGroup(grp1Name);

        Assert.IsFalse(location.GroupExists(grp1Name));
    }

    protected static void OpenExistingGroupPathSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        // Create group
        const string grp1Name = "grp1";
        using var grp1 = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));
        const string grp2Name = "grp2";
        using var grp2 = grp1.CreateGroup(grp2Name);
        Assert.IsTrue(grp1.GroupExists(grp2Name));

        Assert.IsTrue(location.GroupPathExists(grp1Name));
        Assert.IsTrue(location.GroupPathExists($"{grp1Name}/{grp2Name}"));

        if (location is H5File)
        {
            // Only files allow rooted path names
            Assert.IsTrue(location.GroupPathExists($"/{grp1Name}"));
            Assert.IsTrue(location.GroupPathExists($"/{grp1Name}/{grp2Name}"));
            Assert.IsFalse(location.GroupPathExists($"/{grp2Name}/{grp1Name}"));
        }

        // Try to open
        using var group2 = location.OpenGroup(grp1Name);
    }

    protected static void OpenNonExistingGroupThrows<T>(H5Location<T> location) where T : H5LocationHandle
    {
        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Try to open
        Assert.ThrowsException<Hdf5Exception>(() => location.OpenGroup("grp2"));
    }

    protected static void CreateGroupEmptyNameFails<T>(H5Location<T> location) where T : H5LocationHandle
    {
        Assert.ThrowsException<Hdf5Exception>(() => H5Group.Create(location, null));
        Assert.ThrowsException<Hdf5Exception>(() => H5Group.Create(location, string.Empty));
        Assert.ThrowsException<Hdf5Exception>(() => location.CreateGroup(null));
        Assert.ThrowsException<Hdf5Exception>(() => location.CreateGroup(string.Empty));
    }

    protected static void CreateDuplicateGroupFails<T>(H5Location<T> location) where T : H5LocationHandle
    {
        using var group = location.CreateGroup("test");
        Assert.ThrowsException<Hdf5Exception>(() => H5Group.Create(location, "test"));
    }

    protected static void GroupExistsReturnsTrueForGroup<T>(H5Location<T> location) where T : H5LocationHandle
    {
        using var group = location.CreateGroup("test");
        Assert.IsTrue(H5Group.Exists(location, "test"));
    }

    protected static void GroupExistsReturnsFalseForNoGroup<T>(H5Location<T> location) where T : H5LocationHandle
    {
        using var group = location.CreateGroup("test");
        Assert.IsFalse(H5Group.Exists(location, "test1"));
    }

    protected static void GroupExistsThrowsForGroupPathName<T>(H5Location<T> location) where T : H5LocationHandle
    {
        using var subgroup = location.CreateGroup("subgroup");

        Assert.IsTrue(H5Group.Exists(location, "subgroup"));

        Assert.ThrowsException<Hdf5Exception>(() => H5Group.Exists(location, "test/subgroup"));
        Assert.ThrowsException<Hdf5Exception>(() => H5Group.Exists(location, "/test"));
    }

    protected static void GroupPathExistsSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        // Create group
        const string grp1Name = "grp1";
        using var grp1 = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));
        const string grp2Name = "grp2";
        using var grp2 = grp1.CreateGroup(grp2Name);
        Assert.IsTrue(grp1.GroupExists(grp2Name));
        const string grp3Name = "grp3";
        using var grp3 = grp2.CreateGroup(grp3Name);
        Assert.IsTrue(grp2.GroupExists(grp3Name));

        Assert.IsTrue(location.GroupPathExists(grp1Name));
        Assert.IsTrue(grp1.GroupPathExists($"{grp2Name}"));
        Assert.IsTrue(grp1.GroupPathExists($"{grp2Name}/{grp3Name}"));
        Assert.IsTrue(grp2.GroupPathExists($"{grp3Name}"));
    }

    protected static void GetChildGroupsSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        var children = location.GetChildNames();
        Assert.IsTrue(!children.Any());

        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Create group
        const string grp2Name = "grp2";
        using var group2 = location.CreateGroup(grp2Name);
        Assert.IsTrue(location.GroupExists(grp2Name));

        // Create group
        const string grp10Name = "grp10";
        using var group10 = location.CreateGroup(grp10Name);
        Assert.IsTrue(location.GroupExists(grp10Name));

        children = location.GetChildNames();
        Assert.IsTrue(children.Count() == 3);

        Assert.IsTrue(children.All(c => c.isGroup));
        Assert.IsTrue(children.Any(c => c.name == grp1Name));
        Assert.IsTrue(children.Any(c => c.name == grp2Name));
        Assert.IsTrue(children.Any(c => c.name == grp10Name));
    }
    #endregion

    #region DataSet

    protected static void CreateDataSetSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        const string ds1Name = "ds1";

        using var type = H5Type.CreateDoubleArrayType(100);
        using var space = H5Space.CreateSimple(1, new ulong[] { 1 }, new ulong[] { 1 });
        using var propertyList = H5PropertyList.Create(H5P.DATASET_CREATE);
        using var ds1 = location.CreateDataSet(ds1Name, type, space, propertyList);
        
        Assert.IsTrue(location.DataSetExists(ds1Name));
    }

    protected static void CreateAndOpenDataSetSucceeds<T>(H5Location<T> location) where T : H5LocationHandle
    {
        const string ds1Name = "ds1";

        using var type = H5Type.CreateDoubleArrayType(100);
        using var space = H5Space.CreateSimple(1, new ulong[] { 1 }, new ulong[] { 1 });
        using var propertyList = H5PropertyList.Create(H5P.DATASET_CREATE);
        {
            using var ds1 = location.CreateDataSet(ds1Name, type, space, propertyList);

            Assert.IsTrue(location.DataSetExists(ds1Name));
        }

        using var ds2 = location.OpenDataSet(ds1Name);
    }


    #endregion
}
