using System;
using System.Collections.Generic;

namespace HDF5Api
{
    public interface IH5TypeAdapter<in TInput> 
    {
        public H5Type GetH5Type();
        public void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
    }
}
