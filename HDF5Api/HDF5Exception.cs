using HDF5Api.NativeMethodAdapters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HDF5Api;

[Serializable]
public sealed class Hdf5Exception : Exception
{
    public Hdf5Exception()
    {
        Errors = H5EAdapter.WalkStack();
    }

    public Hdf5Exception(string message) : base(message)
    {
        Errors = H5EAdapter.WalkStack();
    }

    private Hdf5Exception(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        // TODO: serialize Errors
    }

    public ICollection<H5ErrorInfo> Errors { get; } = Array.Empty<H5ErrorInfo>();

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine(Message);

        foreach (var error in Errors)
        {
            sb.Append(error.ToString());
        }

        return base.ToString();
    }
}