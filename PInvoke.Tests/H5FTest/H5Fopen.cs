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

public partial class H5FTest
{
    [TestMethod]
    public void H5FopenTest1()
    {
        string fname = Path.GetTempFileName();
        hid_t file = H5F.create(fname, H5F.ACC_TRUNC);
        Assert.IsTrue(file >= 0);
        Assert.IsTrue(H5F.close(file) >= 0);

        file = H5F.open(fname, H5F.ACC_RDONLY);
        Assert.IsTrue(file >= 0);
        Assert.IsTrue(H5F.close(file) >= 0);

        file = H5F.open(fname, H5F.ACC_RDWR);
        Assert.IsTrue(file >= 0);
        Assert.IsTrue(H5F.close(file) >= 0);
        File.Delete(fname);
    }

    [TestMethod]
    public void H5FopenTest2()
    {
        Assert.IsFalse(
            H5F.open("", H5F.ACC_RDONLY) >= 0);
    }
}