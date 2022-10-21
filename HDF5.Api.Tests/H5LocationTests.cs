namespace HDF5.Api.Tests;

public abstract class H5LocationTests<TLoc> : H5Test<TLoc> where TLoc : H5LocationTests<TLoc>
{
    #region Groups

    protected static void CreateGroupSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        // Ascii
        const string grp1Name = "grp1";
        const string grp2Name = "grp2";

        using var grp1 = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        using var grp2 = grp1.CreateGroup(grp2Name);
        Assert.IsTrue(grp1.GroupExists(grp2Name));

        Assert.IsTrue(location.GroupPathExists($"{grp1Name}/{grp2Name}"));

        // UTF-8
        const string grp3Name = "ᚪᚾᚷᛁᚠ᛫ᚻᛖ᛫ᚹᛁᛚᛖ᛫ᚠᚩᚱ᛫ᛞᚱᛁᚻ";
        const string grp4Name = "Χαρακτηριστικό";

        using var grp3 = location.CreateGroup(grp3Name);
        Assert.IsTrue(location.GroupExists(grp3Name));

        using var grp4 = grp3.CreateGroup(grp4Name);
        Assert.IsTrue(grp3.GroupExists(grp4Name));

        Assert.IsTrue(location.GroupPathExists($"{grp3Name}/{grp4Name}"));
    }

    protected static void CreateAndOpenGroupSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Try to open
        using var group2 = location.OpenGroup(grp1Name);
    }

    protected static void CreateOpenDeleteGroupSucceeds<T>(H5Location<T> location) where T : H5Object<T>
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

    protected static void OpenExistingGroupPathSucceeds<T>(H5Location<T> location) where T : H5Object<T>
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

    protected static void OpenNonExistingGroupThrows<T>(H5Location<T> location) where T : H5Object<T>
    {
        // Create group
        const string grp1Name = "grp1";
        using var group = location.CreateGroup(grp1Name);
        Assert.IsTrue(location.GroupExists(grp1Name));

        // Try to open
        Assert.ThrowsException<Hdf5Exception>(() => location.OpenGroup("grp2"));
    }

    protected static void CreateGroupEmptyNameFails<T>(H5Location<T> location) where T : H5Object<T>
    {
        Assert.ThrowsException<ArgumentNullException>(() => location.CreateGroup(null!));
        Assert.ThrowsException<ArgumentException>(() => location.CreateGroup(string.Empty));
    }

    protected static void CreateDuplicateGroupFails<T>(H5Location<T> location) where T : H5Object<T>
    {
        using var group = location.CreateGroup("test");
        Assert.ThrowsException<Hdf5Exception>(() => location.CreateGroup("test"));
    }

    protected static void GroupExistsReturnsTrueForGroup<T>(H5Location<T> location) where T : H5Object<T>
    {
        using var group = location.CreateGroup("test");
        Assert.IsTrue(location.GroupExists("test"));
    }

    protected static void GroupExistsReturnsFalseForNoGroup<T>(H5Location<T> location) where T : H5Object<T>
    {
        using var group = location.CreateGroup("test");
        Assert.IsFalse(location.GroupExists("test1"));
    }

    protected static void GroupExistsThrowsForGroupPathName<T>(H5Location<T> location) where T : H5Object<T>
    {
        using var subgroup = location.CreateGroup("subgroup");

        Assert.IsTrue(location.GroupExists("subgroup"));

        Assert.ThrowsException<Hdf5Exception>(() => location.GroupExists("test/subgroup"));
        Assert.ThrowsException<Hdf5Exception>(() => location.GroupExists("/test"));
    }

    protected static void GroupPathExistsSucceeds<T>(H5Location<T> location) where T : H5Object<T>
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

    protected static void GetChildGroupsSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        var groupNames = location.GroupNames;
        Assert.IsTrue(!groupNames.Any());

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

        groupNames = location.GroupNames;
        Assert.IsTrue(groupNames.Count() == 3);

        Assert.IsTrue(groupNames.Any(c => c == grp1Name));
        Assert.IsTrue(groupNames.Any(c => c == grp2Name));
        Assert.IsTrue(groupNames.Any(c => c == grp10Name));
    }

    protected static void EnumerateGroupsSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        var groupNames = location.GroupNames;
        Assert.IsTrue(!groupNames.Any());

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

        List<string> groupNames2 = new();
        location.Enumerate((H5Group g) => groupNames2.Add(g.Name));

        // Since group name is a full path '/grp1/grp2/grp3' we extract the 'filename' to compare with
        Assert.IsTrue(location.GroupNames.SequenceEqual(groupNames2.Select(g => Path.GetFileNameWithoutExtension(g))));
    }
    #endregion

    #region DataSet

    protected static void CreateDataSetSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        const string ds1Name = "ds1";

        using var type = H5Type.CreateDoubleArrayType(100);
        using var space = H5Space.Create(new Dimension(1, 1));
        using var ds1 = location.CreateDataSet(ds1Name, type, space);

        Assert.IsTrue(location.DataSetExists(ds1Name));
    }

    protected static void CreateAndOpenDataSetSucceeds<T>(H5Location<T> location) where T : H5Object<T>
    {
        const string ds1Name = "ds1";

        using var type = H5Type.CreateDoubleArrayType(100);
        using var space = H5Space.Create((1, 1)); // use value tuple variant

        using var ds1 = location.CreateDataSet(ds1Name, type, space);
        Assert.IsTrue(location.DataSetExists(ds1Name));

        using var ds2 = location.OpenDataSet(ds1Name);
    }

    #endregion
}
