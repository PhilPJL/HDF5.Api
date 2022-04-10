﻿using System;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5 (Global) API.
    /// </summary>
    public static class H5Global
    {
        public static Version GetLibraryVersion()
        {
            uint major = 0;
            uint minor = 0;
            uint revision = 0;
            var err = H5.get_libversion(ref major, ref minor, ref revision);
            H5Handle.AssertError(err);
            return new Version((int)major, (int)minor, 0, (int)revision);
        }
    }
}
