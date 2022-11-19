using HDF5.Api.Disposables;
using HDF5.Api.H5Types;
using System.Collections.Generic;
using System.Linq;

namespace HDF5.Api;

/// <summary>
///     Base class for implementing a custom adaptor/converter to format an instance of a C# type into a blittable struct
///     for use in an HDF5 dataset
/// </summary>
/// <typeparam name="TInput"></typeparam>
internal abstract class H5TypeAdapter<TInput> : IH5TypeAdapter<TInput>
{
    public abstract H5Type GetH5Type();

    public abstract void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
}

/// <summary>
///     Base class for implementing a custom adaptor/converter to format an instance of C# type into a blittable struct for
///     use in an HDF5 dataset
/// </summary>
/// <remarks>
///     It is required to implement the <see cref="Convert(TInput)" /> method to convert <typeparamref name="TInput" /> to
///     <typeparamref name="TOutput" />, and
///     to implement the <see cref="H5TypeAdapter{TInput}.GetH5Type" /> method to provide a matching H5 type definition.
/// </remarks>
internal abstract class H5TypeAdapter<TInput, TOutput> : H5TypeAdapter<TInput>
{
    protected abstract TOutput Convert(TInput source);

    public override void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
    {
        // convert input to Array of struct
        var records = inputRecords.Select(Convert).ToArray();
        // pin
        using var pinnedRecords = new PinnedObject(records);
        // write to HDF5
        write(pinnedRecords);
        //unsafe
        //{
        //    void* fixedRecords = &records[0];
        //    {
        //        write(new IntPtr(fixedRecords));
        //    }
        //}
    }
}
