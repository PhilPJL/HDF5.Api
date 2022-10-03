using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Interface that needs to be implemented by an AttributeWriter.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IH5AttributeWriter<TInput> : IDisposable
{
    void Write(ICollection<TInput> recordsChunk);
    long RowsWritten { get; }
}
