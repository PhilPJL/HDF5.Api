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

public partial class H5TTest
{
    [TestMethod]
    public void H5Tcommit_anonTest1()
    {
        hid_t dtype = H5T.copy(H5T.IEEE_F64LE);
        Assert.IsTrue(dtype >= 0);
        Assert.IsTrue(H5T.commit_anon(m_v0_test_file, dtype) >= 0);
        // can't commit twice
        Assert.IsFalse(H5T.commit_anon(m_v0_test_file, dtype) >= 0);
        // can't commit to different files
        Assert.IsFalse(H5T.commit_anon(m_v2_test_file, dtype) >= 0);
        Assert.IsTrue(H5T.close(dtype) >= 0);
    }

    [TestMethod]
    public void H5Tcommit_anonTest2()
    {
        // can't commit pre-defined types
        Assert.IsFalse(
            H5T.commit_anon(m_v0_test_file, H5T.IEEE_F64BE) >= 0);
        Assert.IsFalse(
            H5T.commit_anon(m_v2_test_file, H5T.IEEE_F64BE) >= 0);
    }

    [TestMethod]
    public void H5Tcommit_anonTest3()
    {
        Assert.IsFalse(
            H5T.commit_anon(m_v0_test_file,
            Utilities.RandomInvalidHandle()) >= 0);
        Assert.IsFalse(
            H5T.commit_anon(Utilities.RandomInvalidHandle(),
            Utilities.RandomInvalidHandle()) >= 0);
    }
}