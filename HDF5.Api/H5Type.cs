using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5T (Type) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_t.html"/>
/// </summary>
public class H5Type : H5Object<H5Type>, IEquatable<H5Type>
{
    internal H5Type(long handle) : base(handle, HandleType.Type, H5TAdapter.Close) { }

    private H5Type(long handle, Action<H5Type>? closer) : base(handle, HandleType.Type, closer) { }

    public bool IsEqualTo([AllowNull] H5Type? other)
    {
        if(other is null) { return false; }

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

    public static H5Type GetNativeType<T>() where T : unmanaged
    {
        long nativeHandle = H5TAdapter.GetNativeType<T>();

        return new H5Type(nativeHandle, null);
    }

    public H5Type Insert([DisallowNull] string name, int offset, [DisallowNull] H5Type dataType)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(dataType);

        H5TAdapter.Insert(this, name, new IntPtr(offset), dataType);
        return this;
    }

    public H5Type Insert([DisallowNull] string name, IntPtr offset, [DisallowNull] H5Type dataType)
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
        H5TAdapter.Insert(this, name, offset, GetNativeType<TP>());
        return this;
    }

    public bool IsVariableLengthString()
    {
        return H5TAdapter.IsVariableLengthString(this);
    }

    public H5Class GetClass()
    {
        return H5TAdapter.GetClass(this);
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

    public static H5Type CreateFixedLengthStringType(int length)
    {
        return H5TAdapter.CreateFixedLengthStringType(length);
    }

    internal void SetCharacterSet(CharacterSet cset)
    {
        H5TAdapter.SetCharacterSet(this, cset);
    }

    internal CharacterSet GetCharacterSet()
    {
        return H5TAdapter.GetCharacterSet(this);
    }

    internal void SetPadding(StringPadding padding)
    {
        H5TAdapter.SetPadding(this, padding);
    }

    internal void SetSize(int size) 
    {
        H5TAdapter.SetSize(this, size);
    }
}
