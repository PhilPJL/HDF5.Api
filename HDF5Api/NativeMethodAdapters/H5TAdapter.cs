﻿using System.Linq;
using static HDF5Api.NativeMethods.H5T;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5TAdapter
{
    public static void Close(H5Type type)
    {
        int err = close(type);

        err.ThrowIfError(nameof(close));
    }

    public static H5Class GetClass(H5Type typeId)
    {
        return (H5Class)get_class(typeId);
    }

    public static H5Type CreateCompoundType(int size)
    {
        long h = create((class_t)H5Class.Compound, new ssize_t(size));
        h.ThrowIfInvalidHandleValue(nameof(create));
        return new H5Type(h);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" />
    /// </summary>
    public static H5Type CreateCompoundType<T>() where T : struct
    {
        int size = Marshal.SizeOf<T>();
        return CreateCompoundType(size);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    public static H5Type CreateCompoundType<T>(int extraSpace) where T : struct
    {
        int size = Marshal.SizeOf<T>() + extraSpace;
        return CreateCompoundType(size);
    }

    public static H5Type CreateByteArrayType(params long[] dims)
    {
        long h = array_create(NATIVE_B8, (uint)dims.Length, dims.Cast<ulong>().ToArray());
        h.ThrowIfInvalidHandleValue(nameof(array_create));
        return new H5Type(h);
    }

    public static H5Type CreateDoubleArrayType(int size)
    {
        long h = array_create(NATIVE_DOUBLE, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue(nameof(array_create));
        return new H5Type(h);
    }

    public static H5Type CreateFloatArrayType(int size)
    {
        long h = array_create(NATIVE_FLOAT, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue(nameof(array_create));
        return new H5Type(h);
    }

    public static H5Type CreateVariableLengthByteArrayType()
    {
        long h = vlen_create(NATIVE_B8);
        h.ThrowIfInvalidHandleValue(nameof(vlen_create));
        return new H5Type(h);
    }

    public static H5Type CreateFixedLengthStringType(int length)
    {
        long h = copy(C_S1);
        h.ThrowIfInvalidHandleValue(nameof(copy));
        int err = set_size(h, new ssize_t(length));
        err.ThrowIfError(nameof(set_size));
        return new H5Type(h);
    }

    public static void Insert(H5Type typeId, string name, ssize_t offset, long nativeTypeId)
    {
        int err = insert(typeId, name, offset, nativeTypeId);
        err.ThrowIfError(nameof(insert));
    }

    public static void Insert(H5Type typeId, string name, ssize_t offset, H5Type dataTypeId)
    {
        int err = insert(typeId, name, offset, dataTypeId);
        err.ThrowIfError(nameof(insert));
    }

    public static bool IsVariableLengthString(H5Type typeId)
    {
        int err = is_variable_str(typeId);
        err.ThrowIfError(nameof(is_variable_str));
        return err > 0;
    }
}

public enum H5Class
{
    None = -1,
    Integer = 0,
    Float = 1,
    Time = 2,
    String = 3,
    BitField = 4,
    Opaque = 5,
    Compound = 6,
    Reference = 7,
    Enum = 8,
    VariableLength = 9,
    Array = 10,
    NClasses 
}
