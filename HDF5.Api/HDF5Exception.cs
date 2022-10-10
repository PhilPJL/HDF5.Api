using HDF5Api.NativeMethodAdapters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HDF5Api;

[Serializable]
public sealed class Hdf5Exception : Exception
{
    public Hdf5Exception()
    {
        H5Errors = H5Error.WalkStack();
    }

    public Hdf5Exception(string message) : base(message)
    {
        H5Errors = H5Error.WalkStack();
    }

    private Hdf5Exception(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        // TODO: serialize H5Errors
    }

    public ICollection<H5ErrorInfo> H5Errors { get; } = Array.Empty<H5ErrorInfo>();

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine(Message);

        foreach (var error in H5Errors)
        {
            sb.Append(error.ToString());
        }

        return sb.ToString();
    }
}