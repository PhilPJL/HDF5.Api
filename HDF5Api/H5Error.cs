using System;

namespace HDF5Api;

public static partial class H5Error
{
    #region SetAuto

    /// <summary>
    /// Turns automatic error printing on or off.
    /// </summary>
    /// <param name="estack_id">Error stack identifier.</param>
    /// <param name="func">Function to be called upon an error condition.</param>
    /// <param name="client_data">Data passed to the error function.</param>
    /// <returns>Returns a non-negative value on success; otherwise returns
    /// a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Eset_auto2")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t H5Eset_auto2
        (hid_t estack_id, auto_t? func, IntPtr client_data);

    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        int err = H5Eset_auto2(H5E.DEFAULT, null, IntPtr.Zero);
        err.ThrowIfError(nameof(H5Eset_auto2));
    }

    /// <summary>
    /// Callback for error handling.
    /// </summary>
    /// <param name="estack">Error stack identifier</param>
    /// <param name="client_data">Pointer to client data in the format
    /// expected by the user-defined function.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate herr_t auto_t(hid_t estack, IntPtr client_data);

    #endregion
}
