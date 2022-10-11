﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */




namespace UnitTests;

public partial class H5DTest
{
    [TestMethod]
    public void H5Dget_space_statusTest1()
    {
        hsize_t[] dims = { 1024, 2048 };
        hid_t space = H5S.create_simple(3, dims, null);

        hid_t dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
            space);
        Assert.IsTrue(dset >= 0);
        H5D.space_status_t status = H5D.space_status_t.ERROR;
        Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
        Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
        Assert.IsTrue(H5D.close(dset) >= 0);

        dset = H5D.create(m_v2_test_file, "dset", H5T.STD_I16LE,
            space);
        Assert.IsTrue(dset >= 0);
        status = H5D.space_status_t.ERROR;
        Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
        Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
        Assert.IsTrue(H5D.close(dset) >= 0);

        Assert.IsTrue(H5S.close(space) >= 0);
    }

    [TestMethod]
    public void H5Dget_space_statusTest2()
    {
        hid_t dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
            m_space_null);
        Assert.IsTrue(dset >= 0);
        H5D.space_status_t status = H5D.space_status_t.ERROR;
        Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
        Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
        Assert.IsTrue(H5D.close(dset) >= 0);

        dset = H5D.create(m_v2_test_file, "dset", H5T.STD_I16LE,
            m_space_null);
        Assert.IsTrue(dset >= 0);
        status = H5D.space_status_t.ERROR;
        Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
        Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
        Assert.IsTrue(H5D.close(dset) >= 0);
    }

    [TestMethod]
    public void H5Dget_space_statusTest3()
    {
        H5D.space_status_t status = H5D.space_status_t.ERROR;
        Assert.IsFalse(H5D.get_space_status(Utilities.RandomInvalidHandle(),
            ref status) >= 0);
    }
}