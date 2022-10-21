using System.Collections.Generic;
using System.Diagnostics;
using static HDF5.Api.NativeMethods.H5E;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 error native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_e.html"/>
/// </summary>
internal static class H5EAdapter
{
    internal static void DisableErrorPrinting()
    {
        int err = set_auto(DEFAULT, null!, IntPtr.Zero);
        err.ThrowIfError();
    }

    internal static ICollection<H5ErrorInfo> WalkStack()
    {
        List<H5ErrorInfo> errors = new();

        int err = walk(DEFAULT, direction_t.H5E_WALK_DOWNWARD, Callback, IntPtr.Zero);

        if (err < 0)
        {
            Trace.WriteLine("Error calling H5E.walk");
        }

        return errors;

        herr_t Callback(uint n, ref error_t err_desc, IntPtr client_data)
        {
            try
            {
                errors.Add(new H5ErrorInfo
                (
                    (int)n,
                    (int)err_desc.line,
                    err_desc.func_name,
                    err_desc.file_name,
                    err_desc.desc
                ));

                return 0;
            }
            catch(Exception ex)
            {
                Trace.WriteLine($"Error in WalkStack.Callback: {ex.Message}.");
                return -1;
            }
        }
    }
}