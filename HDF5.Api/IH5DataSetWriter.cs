using System.Collections.Generic;

namespace HDF5.Api;

/// <summary>
///     Interface that needs to be implemented by an IH5DataSetWriter.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IH5DataSetWriter<TInput> : IDisposable
{
    void Write(IEnumerable<TInput> recordsChunk);
    long RowsWritten { get; }
}
