using System;
using System.Collections.Generic;

namespace HDF5Api
{
    /// <summary>
    /// Interface that needs to be implemented by the H5TypeAdapter used by the IH5DataWriter.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IH5TypeAdapter<in TInput>
    {
        public H5Type GetH5Type();
        public void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
    }
}
