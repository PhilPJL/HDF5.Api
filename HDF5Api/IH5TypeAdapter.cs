using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Interface that needs to be implemented by the H5TypeAdapter used by the IH5DataWriter.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IH5TypeAdapter<TInput>
{
    H5Type GetH5Type();
    void Write(Action<IntPtr> write, ICollection<TInput> inputRecords);
}
