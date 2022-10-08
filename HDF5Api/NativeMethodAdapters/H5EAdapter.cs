using System.Collections.Generic;
using System.Diagnostics;
using static HDF5Api.NativeMethods.H5E;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5EAdapter
{
    public static void SetAutoOff()
    {
        int err = set_auto(DEFAULT, null!, IntPtr.Zero);
        err.ThrowIfError();
    }

    public static ICollection<H5ErrorInfo> WalkStack()
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
                Description = err_desc.desc,
                Filename = err_desc.file_name,
                FunctionName = err_desc.func_name,
                LineNumber = (int)err_desc.line
            });

            return 0;
        }
    }
}

public record struct H5ErrorInfo
(
    int LineNumber,
    string FunctionName,
    string Filename,
    string Description
);