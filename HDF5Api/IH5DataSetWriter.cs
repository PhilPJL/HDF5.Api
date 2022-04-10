using System;
using System.Collections.Generic;

namespace HDF5Api
{
    /// <summary>
    /// Interface that needs to be implemented by an IH5DataSetWriter.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IH5DataSetWriter<in TInput> : IDisposable
    {
        public void WriteChunk(IEnumerable<TInput> recordsChunk);
        public int RowsWritten { get; }
    }
}
