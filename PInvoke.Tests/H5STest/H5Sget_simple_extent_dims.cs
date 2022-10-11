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
    public void H5Sget_simple_extent_dimsTest1()
    {
        hsize_t[] dims = { 1, 2, 3 };
        hid_t space = -1;
        hsize_t[] dims_out = new hsize_t[3];

        space = H5S.create_simple(dims.Length, dims, dims);
        Assert.IsTrue(
            H5S.get_simple_extent_dims(space, (ulong[])null, null) == 3);

        Assert.IsTrue(
            H5S.get_simple_extent_dims(space, dims_out, null) == 3);

        Assert.IsTrue(dims_out[2] == 3);

        Assert.IsTrue(
            H5S.get_simple_extent_dims(space, null, dims_out) == 3);

        Assert.IsTrue(dims_out[0] == 1);

        Assert.IsTrue(space > 0);
        Assert.IsTrue(H5S.close(space) >= 0);
    }

    [TestMethod]
    public void H5Sget_simple_extent_dimsTest2()
    {
        hsize_t[] dims = { 1, 2, 3 };
        hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
        hid_t space = -1;
        hsize_t[] dims_out = new hsize_t[3];

        space = H5S.create_simple(dims.Length, dims, max_dims);
        Assert.IsTrue(
        H5S.get_simple_extent_dims(space, (ulong[])null, null) == 3);

        Assert.IsTrue(
            H5S.get_simple_extent_dims(space, dims_out, null) == 3);
        Assert.IsTrue(dims_out[0] == 1);

        Assert.IsTrue(
            H5S.get_simple_extent_dims(space, null, dims_out) == 3);
        Assert.IsTrue(dims_out[0] == H5S.UNLIMITED);

        Assert.IsTrue(space > 0);
        Assert.IsTrue(H5S.close(space) >= 0);
    }
}
