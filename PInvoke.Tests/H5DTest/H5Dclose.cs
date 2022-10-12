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
    public void H5DcloseTest1()
    {
        hid_t dset = H5D.create_anon(m_v0_test_file, H5T.STD_I16LE,
            m_space_null);
        Assert.IsTrue(dset >= 0);
        Assert.IsTrue(H5D.close(dset) >= 0);

        dset = H5D.create_anon(m_v2_test_file, H5T.STD_I16LE, m_space_null);
        Assert.IsTrue(dset >= 0);
        Assert.IsTrue(H5D.close(dset) >= 0);
    }

    [TestMethod]
    public void H5DcloseTest2()
    {
        Assert.IsFalse(H5D.close(Utilities.RandomInvalidHandle()) >= 0);
    }
}