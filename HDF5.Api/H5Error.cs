using System.Collections.Generic;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5E (Error) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_e.html"/>
/// </summary>
public static class H5Error
{
    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        H5EAdapter.DisableErrorPrinting();
    }

    public static ICollection<H5ErrorInfo> WalkStack()
    {
        return H5EAdapter.WalkStack();
    }
}

public 
#if NET7_0_OR_GREATER
readonly 
#endif
record struct H5ErrorInfo
(
    int Number,
    int LineNumber,
    string FunctionName,
    string Filename,
    string Description
)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(nameof(H5ErrorInfo));
        sb.AppendLine("{");
        sb.AppendLine($"\t{nameof(Number)} = {Number}");
        sb.AppendLine($"\t{nameof(LineNumber)} = {LineNumber}");
        sb.AppendLine($"\t{nameof(FunctionName)} = {FunctionName}");
        sb.AppendLine($"\t{nameof(Filename)} = '{Filename}'");
        sb.AppendLine($"\t{nameof(Description)} = '{Description}'");
        sb.AppendLine("}");
        return sb.ToString();
    }
}