using System.Collections.Generic;
using HDF5.Api.H5Types;

namespace HDF5.Api;

/// <summary>
///     Interface that needs to be implemented by the H5TypeAdapter used by the IH5DataWriter.
/// </summary>
/// <typeparam name="TInput"></typeparam>
internal interface IH5TypeAdapter<in TInput>
{
    H5Type GetH5Type();
    void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
}
