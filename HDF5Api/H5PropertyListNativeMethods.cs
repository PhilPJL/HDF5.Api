using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

internal static partial class H5PropertyListNativeMethods
    {
        #region Close

        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pclose")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static partial int H5PClose(long handle);

        public static void Close(H5PropertyList type)
        {
            int err = H5PClose(type);
            // TODO: get additional error info 
            err.ThrowIfError("H5Tclose");
        }

        #endregion

        public static H5PropertyList Create(Handle classId)
        {
            Handle h = H5P.create(classId);

            h.ThrowIfInvalidHandleValue("H5P.create");

            return new H5PropertyList(h);
        }

        public static void SetChunk(H5PropertyList propertyList, int rank, ulong[] dims)
        {
            int err = H5P.set_chunk(propertyList, rank, dims);

            err.ThrowIfError("H5P.set_chunk");
        }

        public static void EnableDeflateCompression(H5PropertyList propertyList, uint level)
        {
            int err = H5P.set_deflate(propertyList, level);

            err.ThrowIfError("H5P.set_deflate");
        }

        /// <summary>
        /// Get copy of property list used to create the data-set.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static H5PropertyList GetCreationPropertyList(H5DataSet dataSet)
        {
            Handle h = H5D.get_create_plist(dataSet);
            h.ThrowIfInvalidHandleValue("H5D.get_create_plist");
            return new H5PropertyList(h);
        }

    }


