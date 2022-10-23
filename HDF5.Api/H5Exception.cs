using System.Collections.Generic;
using System.Linq;

namespace HDF5.Api;

[Serializable]
public sealed class H5Exception : Exception
{
    public H5Exception(string message) : base(message)
    {
        H5Errors = H5Error.WalkStack();
    }

    public override string Message
    {
        get
        {
            if (H5Errors.Count > 0)
            {
                return string.Join("→", H5Errors
                    .OrderByDescending(e => e.Number)
                    .Select(e => e.Description));
            }

            return base.Message;
        }
    }

    public ICollection<H5ErrorInfo> H5Errors { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var error in H5Errors)
        {
            sb.Append(error.ToString());
        }

        sb.AppendLine(Message);

        return sb.ToString();
    }
}