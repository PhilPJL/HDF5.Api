using System;
using System.Collections.Generic;

namespace HDF5Api
{
    public interface IH5DataSetWriter<in TInput, out TOutput> : IDisposable where TOutput : struct
    {
        public void Write(IEnumerable<TInput> recordsChunk);
        public int CurrentPosition { get; }
    }
}
