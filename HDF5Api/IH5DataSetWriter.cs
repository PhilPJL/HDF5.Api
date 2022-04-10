using System;
using System.Collections.Generic;

namespace HDF5Api
{
    public interface IH5DataSetWriter<in TInput> : IDisposable
    {
        public void WriteChunk(IEnumerable<TInput> recordsChunk);
        public int CurrentPosition { get; }
    }
}
