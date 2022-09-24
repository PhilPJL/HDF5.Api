using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

internal static partial class H5FileNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fclose")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int H5Fclose(long handle);

    public static void Close(H5File file)
    {
        int err = H5Fclose(file);
        // TODO: get additional error info 
        err.ThrowIfError("H5Fclose");
    }

    #endregion

    /// <summary>
    ///     Create a new file. Truncates existing file.
    /// </summary>
    public static H5File Create(string path)
    {
        long h = H5F.create(path, H5F.ACC_TRUNC);

        h.ThrowIfInvalidHandleValue("H5F.create");

        return new H5File(h);
    }

    public static H5File Open(string path, bool readOnly)
    {
        long h = H5F.open(path, readOnly ? H5F.ACC_RDONLY : H5F.ACC_RDWR);

        h.ThrowIfInvalidHandleValue("H5F.open");

        return new H5File(h);
    }

    public static long GetObjectCount(H5File file, H5ObjectTypes types = H5ObjectTypes.All)
    {
        return H5F.get_obj_count(file, (uint)types);
    }
}
