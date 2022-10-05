using CommunityToolkit.Diagnostics;
using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5T (Type) API.
/// </summary>
public class H5Type : H5Object<H5Type>
{
    internal H5Type(long handle) : base(handle, H5TAdapter.Close)
    {
    }

    private H5Type(long handle, Action<H5Type>? closer) : base(handle, closer) { }

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

}
