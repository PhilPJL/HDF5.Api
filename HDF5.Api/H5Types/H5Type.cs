﻿using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

/// <summary>
///     <para>.NET wrapper for H5T (Type) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_t.html"/>
/// </summary>
public class H5Type : H5ObjectWithAttributes<H5Type>, IEquatable<H5Type>
{
    internal H5Type(long handle) : base(handle, HandleType.Type, H5TAdapter.Close) { }

    //private H5Type(long handle, Action<H5Type>? closer) : base(handle, HandleType.Type, closer) { }

    //internal static H5Type CreateNonTracked(long handle) => new(handle, null);

    #region Equality and hashcode

    public bool IsEqualTo([AllowNull] H5Type? other)
    {
        if (other is null) { return false; }

        return H5TAdapter.AreEqual(this, other);
    }

#if NET7_0_OR_GREATER
    public bool Equals([AllowNull] H5Type? other)
#else
    public bool Equals([AllowNull] H5Type other)
#endif
    {
        return IsEqualTo(other);
    }

#if NET7_0_OR_GREATER
    public override bool Equals(object? obj)
#else
    public override bool Equals(object obj)
#endif
    {
        if (obj is not H5Type other) { return false; }

        return Equals(other);
    }

    public override int GetHashCode()
    {
        // Use the handle value which will be unique anyway - hopefully
        return HashCode.Combine((long)this);
    }

    #endregion

    /// <summary>
    /// Gets the equivalent native type for a primitive .NET type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="H5Exception"></exception>
    public static H5Type GetEquivalentNativeType<T>() //where T : unmanaged
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<T>();
    }

    /// <summary>
    /// Gets the equivalent native type for an H5 type
    /// </summary>
    /// <exception cref="H5Exception"></exception>
    public static H5Type GetEquivalentNativeType(H5Type type)
    {
        return H5TAdapter.GetEquivalentNativeType(type);
    }

    public H5Type Insert([DisallowNull] string name, int offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, new ssize_t(offset), dataType);
        return this;
    }

    public H5Type Insert([DisallowNull] string name, ssize_t offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, offset, dataType);
        return this;
    }

    public H5Type Insert<TS, TP>([DisallowNull] string name) where TS : struct where TP : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        var offset = Marshal.OffsetOf<TS>(name);
        using var type = GetEquivalentNativeType<TP>();
        H5TAdapter.Insert(this, name, offset, type);
        return this;
    }

    public static H5Type CreateDoubleArrayType(int size)
    {
        return H5TAdapter.CreateDoubleArrayType(size);
    }

    public static H5Type CreateCompoundType(int size)
    {
        return H5TAdapter.CreateCompoundType(size);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" />
    /// </summary>
    public static H5Type CreateCompoundType<T>() where T : unmanaged
    {
        return H5TAdapter.CreateCompoundType<T>();
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    public static H5Type CreateCompoundType<T>(int extraSpace) where T : unmanaged
    {
        return H5TAdapter.CreateCompoundType<T>(extraSpace);
    }

    public static H5Type CreateByteArrayType(int size)
    {
        return H5TAdapter.CreateByteArrayType(size);
    }

    public static H5Type CreateFloatArrayType(int size)
    {
        return H5TAdapter.CreateFloatArrayType(size);
    }

    public static H5Type CreateVariableLengthByteArrayType()
    {
        return H5TAdapter.CreateVariableLengthByteArrayType();
    }

    public static H5EnumType<T> CreateEnumType<T>() where T : Enum // unmanaged
    {
        return H5TAdapter.CreateEnumType<T>();
    }

    public int Size
    {
        get => H5TAdapter.GetSize(this);
        set => H5TAdapter.SetSize(this, value);
    }

    public bool Committed => H5TAdapter.GetCommitted(this);

    public DataTypeClass GetClass() => H5TAdapter.GetClass(this);

    internal static H5DataTypeCreationPropertyList CreateCreationPropertyList()
    {
        return H5TAdapter.CreateCreationPropertyList();
    }

    internal static H5DataTypeAccessPropertyList CreateAccessPropertyList()
    {
        return H5TAdapter.CreateAccessPropertyList();
    }

    public string Name => H5IAdapter.GetName(this);

    public bool IsVariableLengthString()
    {
        return H5TAdapter.IsVariableLengthString(this);
    }

    internal int NumberOfMembers => H5TAdapter.GetNumberOfMembers(this);
}

public class H5Type<T> : H5Type
{
    internal H5Type(long handle) : base(handle)
    {
    }
}