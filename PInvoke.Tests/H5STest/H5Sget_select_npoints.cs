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

public partial class H5STest
{
    [TestMethod]
    public void H5Sget_select_npointsTest1()
    {
        hsize_t[] dims = { 1, 2, 3 };
        hid_t space = H5S.create_simple(dims.Length, dims, dims);
        Assert.IsTrue(space > 0);
        Assert.IsTrue(H5S.select_all(space) >= 0);
        Assert.IsTrue(H5S.get_select_npoints(space) == 6);
        Assert.IsTrue(H5S.close(space) >= 0);
    }

    [TestMethod]
    public void H5Sget_select_npointsTest2()
    {
        Assert.IsFalse(
            H5S.get_select_npoints(Utilities.RandomInvalidHandle()) >= 0);
    }

    [TestMethod]
    public void H5Sget_select_npointsTest3()
    {
        hid_t space = H5S.create(H5S.class_t.NULL);
        Assert.IsTrue(space > 0);
        Assert.IsTrue(H5S.select_all(space) >= 0);
        Assert.IsTrue(H5S.get_select_npoints(space) == 0);
        Assert.IsTrue(H5S.close(space) >= 0);
    }

    [TestMethod]
    public void H5Sget_select_npointsTest4()
    {
        hid_t space = H5S.create(H5S.class_t.SCALAR);
        Assert.IsTrue(space > 0);
        Assert.IsTrue(H5S.select_all(space) >= 0);
        Assert.IsTrue(H5S.get_select_npoints(space) == 1);
        Assert.IsTrue(H5S.close(space) >= 0);
    }
}
