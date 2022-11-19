using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5.Api.H5Attributes;

public class H5PrimitiveAttribute<T> : H5Attribute<T, H5PrimitiveAttribute<T>, H5PrimitiveType<T>> //where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
    }

    public override H5PrimitiveType<T> GetAttributeType()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        return H5AAdapter.GetType(this, static h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        using var type = GetAttributeType();
        using var expectedType = H5PrimitiveType<T>.Create();
        return H5AAdapter.ReadImpl<T>(this, type, expectedType);
    }

    public override IEnumerable<T> ReadCollection()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        using var type = GetAttributeType();
        using var expectedType = H5PrimitiveType<T>.Create();

        return H5AAdapter.ReadCollectionImpl<T>(this, type, expectedType);
    }

    public override void Write([DisallowNull] T value) //where T : unmanaged
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
        Guard.IsNotNull(value);

        using var type = GetAttributeType();
        H5AAdapter.Write(this, type, value);
    }

    public static H5PrimitiveAttribute<T> Create(long handle)
    {
        return new H5PrimitiveAttribute<T>(handle);
    }

    public unsafe override void Write([DisallowNull] IEnumerable<T> value) 
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
        Guard.IsNotNull(value);

        using var type = GetAttributeType();
        using var space = GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO
        // if (type.IsVariableLength) 
        using var spanOwner = SpanOwner<byte>.Allocate(StorageSize);

        // This is poor performance compared to H5UnmanagedPrimitiveAttribute
        // TODO: optimise - memory copy entire array

        //var span = value.ToArray().AsSpan();
        int i = 0;
        foreach(var t in value)
        {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            var span = new Span<byte>(&t, sizeof(T));
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            span.CopyTo(spanOwner.Span.Slice(i++ * type.Size, type.Size));
        }

        H5AAdapter.Write(this, type, spanOwner.Span);
    }
}

public class H5UnmanagedPrimitiveAttribute<T> : H5PrimitiveAttribute<T> where T : unmanaged
{
    internal H5UnmanagedPrimitiveAttribute(long handle) : base(handle)
    {
    }

    public new static H5UnmanagedPrimitiveAttribute<T> Create(long handle)
    {
        return new H5UnmanagedPrimitiveAttribute<T>(handle);
    }

    public unsafe override void Write([DisallowNull] IEnumerable<T> value)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
        Guard.IsNotNull(value);

        using var type = GetAttributeType();
        using var space = GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        var span = (value as T[] ?? value.ToArray()).AsSpan();

        H5AAdapter.Write(this, type, MemoryMarshal.Cast<T, byte>(span));
    }
}