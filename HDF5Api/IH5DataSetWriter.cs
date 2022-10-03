using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Interface that needs to be implemented by an IH5DataSetWriter.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IH5DataSetWriter<TInput> : IDisposable
{
    void Write(ICollection<TInput> recordsChunk);
    long RowsWritten { get; }
}
