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
            Debug.WriteLine("Error calling H5E.walk");
        }

        return errors;

        herr_t Callback(uint n, ref error_t err_desc, IntPtr client_data)
        {
            errors.Add(new H5ErrorInfo
            {
                Number = (int)n,
                Description = err_desc.desc,
                Filename = err_desc.file_name,
                FunctionName = err_desc.func_name,
                LineNumber = (int)err_desc.line
            });

            return 0;
        }
    }
}