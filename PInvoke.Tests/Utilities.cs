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

using HDF5.Api;
using System.Diagnostics;

namespace UnitTests;

internal static class Utilities
{
    public static void DisableErrorPrinting()
    {
        _ = H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
    }

    private readonly static object _lock = new object();

    /// <summary>
    /// Create a temporary HDF5 file IN MEMORY and return its name and
    /// a file handle.
    /// </summary>
    public static hid_t H5TempFile(ref string fileName,
        H5F.libver_t version = H5F.libver_t.LATEST,
        bool backing_store = false)
    {
        // TODO: this intermittently fails with 'can't truncate file, already open'.
        int attempts = 0;
        while (true)
        {
            lock (_lock)
            {
                try
                {

                    hid_t fapl = H5P.create(H5P.FILE_ACCESS);
                    if (fapl < 0)
                    {
                        throw new H5Exception("H5P.create failed.");
                    }
                    if (H5P.set_libver_bounds(fapl, version) < 0)
                    {
                        throw new H5Exception("H5P.set_libver_bounds failed.");
                    }
                    // use the core VFD, 64K increments, no backing store
                    if (H5P.set_fapl_core(fapl, new IntPtr(65536),
                        (uint)(backing_store ? 1 : 0)) < 0)
                    {
                        throw new H5Exception("H5P.set_fapl_core failed.");
                    }
                    fileName = Path.GetTempFileName();
                    hid_t file = H5F.create(fileName, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);
                    if (file < 0)
                    {
                        throw new H5Exception("H5F.create failed.");
                    }
                    if (H5P.close(fapl) < 0)
                    {
                        throw new H5Exception("H5P.close failed.");
                    }
                    return file;
                }
                catch(Exception ex)
                {
                    attempts++;

                    Debug.WriteLine(ex.Message);

                    Thread.Sleep(100);

                    if(attempts >= 3)
                    {
                        throw;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Create a temporary HDF5 with SWMR access and return
    /// its name and a file handle.
    /// </summary>
    public static hid_t H5TempFileNoSWMR(ref string fileName)
    {
        hid_t fapl = H5P.create(H5P.FILE_ACCESS);
        if (fapl < 0)
        {
            throw new H5Exception("H5P.create failed.");
        }
        if (H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST) < 0)
        {
            throw new H5Exception("H5P.set_libver_bounds failed.");
        }
        if (H5P.set_fclose_degree(fapl, H5F.close_degree_t.STRONG) < 0)
        {
            throw new H5Exception("H5P.set_fclose_degree failed.");
        }
        fileName = Path.GetTempFileName();
        hid_t file = H5F.create(fileName, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);
        if (file < 0)
        {
            throw new H5Exception("H5F.create failed.");
        }
        if (H5P.close(fapl) < 0)
        {
            throw new H5Exception("H5P.close failed.");
        }
        return file;
    }

    /// <summary>
    /// Create a temporary HDF5 with SWMR access and return
    /// its name and a file handle.
    /// </summary>
    public static hid_t H5TempFileSWMR(ref string fileName)
    {
        hid_t fapl = H5P.create(H5P.FILE_ACCESS);
        if (fapl < 0)
        {
            throw new H5Exception("H5P.create failed.");
        }
        if (H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST) < 0)
        {
            throw new H5Exception("H5P.set_libver_bounds failed.");
        }
        if (H5P.set_fclose_degree(fapl, H5F.close_degree_t.STRONG) < 0)
        {
            throw new H5Exception("H5P.set_fclose_degree failed.");
        }
        fileName = Path.GetTempFileName();
        hid_t file = H5F.create(fileName, H5F.ACC_TRUNC | H5F.ACC_SWMR_WRITE,
            H5P.DEFAULT, fapl);
        if (file < 0)
        {
            throw new H5Exception("H5F.create failed.");
        }
        if (H5P.close(fapl) < 0)
        {
            throw new H5Exception("H5P.close failed.");
        }
        return file;
    }

    /// <summary>
    /// Return a random INVALID handle.
    /// </summary>
    public static hid_t RandomInvalidHandle()
    {
        Random r = new Random();
        return (hid_t)r.Next(Int32.MinValue, -44);
    }
}
