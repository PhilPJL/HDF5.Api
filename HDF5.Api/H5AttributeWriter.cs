using System.Collections.Generic;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.NativeMethods;

namespace HDF5.Api;

public static class H5AttributeWriter
{
    public static IH5AttributeWriter<TInput> CreateAttributeWriter<TInput>
        (IH5Location location, Func<TInput, string> getAttributeName, IH5TypeAdapter<TInput> converter)
    {
        // NOTE: we're only interested in creating attributes currently, not reading them

        return new H5AttributeWriter<TInput>(location, converter, getAttributeName);
    }
}

/// <summary>
///     Implementation of a Compound Type attribute writer.
/// </summary>
/// <remarks>
///     With a suitable <see cref="IH5TypeAdapter{TInput}" /> this writer can be used to writer a collection of
///     <typeparamref name="TInput" /> to a target <see cref="IH5Location" />
/// </remarks>
public class H5AttributeWriter<TInput> : Disposable, IH5AttributeWriter<TInput>
{
    public long RowsWritten { get; private set; }
    private H5Type Type { get; set; }
    private IH5TypeAdapter<TInput> Converter { get; }
    private Func<TInput, string> GetAttributeName { get; }
    private IH5Location Location { get; }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="location">Location to write to.  Could be a file or group.</param>
    /// <param name="converter">Converter to provide <typeparamref name="TInput" /> instances.</param>
    /// <param name="getAttributeName">Func to provide an attribute name for each attribute as it's written.</param>
    public H5AttributeWriter(IH5Location location, IH5TypeAdapter<TInput> converter, Func<TInput, string> getAttributeName)
    {
        Location = location;
        Type = converter.GetH5Type();
        Converter = converter;
        GetAttributeName = getAttributeName;
    }

    public void Write(ICollection<TInput> recordsChunk)
    {
        // Single dimension (rank 1), unlimited length, chunk size.
        using var memorySpace = H5SAdapter.CreateSimple(1);
        using var properyList = H5PAdapter.Create(H5P.ATTRIBUTE_CREATE);

        foreach (var record in recordsChunk)
        {
            // Create the attribute with our record type and chunk size.
            // Create with the name specified by the GetAttributeName function.
            using var attribute =
                Location.CreateAttribute(GetAttributeName(record), Type, memorySpace, properyList);

            // TODO: use Span<TInput>
            Converter.Write(WriteAdaptor(attribute, Type), new [] { record });
 
            RowsWritten += 1;
        }

        // Curry attribute.Write to an Action<IntPtr>
        Action<IntPtr> WriteAdaptor(H5Attribute attribute, H5Type type)
        {
            return buffer => attribute.Write(type, buffer);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !Type.IsDisposed())
        {
            Type.Dispose();
        }
    }
}
