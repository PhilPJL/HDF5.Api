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

public partial class H5SWMRTest
{
    [TestMethod]
    public void H5DflushTestSWMR1()
    {
        hsize_t[] dims = { 6, 6 };
        hsize_t[] maxdims = { 6, H5S.UNLIMITED };
        hsize_t[] chunk_dims = { 2, 5 };
        int[] cbuf = new int[36];

        hid_t dsp = H5S.create_simple(2, dims, maxdims);
        Assert.IsTrue(dsp >= 0);

        hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
        Assert.IsTrue(dcpl >= 0);
        Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk_dims) >= 0);

        hid_t dst = H5D.create(m_v3_test_file_no_swmr, "dset",
            H5T.NATIVE_INT, dsp, H5P.DEFAULT, dcpl);
        Assert.IsTrue(dst >= 0);

        GCHandle hnd = GCHandle.Alloc(cbuf, GCHandleType.Pinned);

        Assert.IsTrue(H5D.write(dst, H5T.NATIVE_INT, H5S.ALL, H5S.ALL,
            H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);

        hnd.Free();

        Assert.IsTrue(H5D.flush(dst) >= 0);

        Assert.IsTrue(H5D.close(dst) >= 0);
        Assert.IsTrue(H5P.close(dcpl) >= 0);
        Assert.IsTrue(H5S.close(dsp) >= 0);
    }

    [TestMethod]
    public void H5DflushTestSWMR2()
    {
        hsize_t[] dims = { 6, 6 };
        hsize_t[] maxdims = { 6, H5S.UNLIMITED };
        hsize_t[] chunk_dims = { 2, 5 };
        int[] cbuf = new int[36];

        hid_t dsp = H5S.create_simple(2, dims, maxdims);
        Assert.IsTrue(dsp >= 0);

        hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
        Assert.IsTrue(dcpl >= 0);
        Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk_dims) >= 0);

        hid_t dst = H5D.create(m_v3_test_file_swmr, "dset",
            H5T.NATIVE_INT, dsp, H5P.DEFAULT, dcpl);
        Assert.IsTrue(dst >= 0);

        GCHandle hnd = GCHandle.Alloc(cbuf, GCHandleType.Pinned);

        Assert.IsTrue(H5D.write(dst, H5T.NATIVE_INT, H5S.ALL, H5S.ALL,
            H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);

        hnd.Free();

        Assert.IsTrue(H5D.flush(dst) >= 0);

        Assert.IsTrue(H5D.close(dst) >= 0);
        Assert.IsTrue(H5P.close(dcpl) >= 0);
        Assert.IsTrue(H5S.close(dsp) >= 0);
    }
}
