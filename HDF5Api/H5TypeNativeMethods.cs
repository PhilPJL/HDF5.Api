using System;
using System.Runtime.InteropServices;
using System.Security;

namespace HDF5Api;

internal static partial class H5TypeNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tclose")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int H5Tclose(long handle);

    public static void Close(H5Type type)
    {
        int err = H5Tclose(type);

        err.ThrowIfError("H5Tclose");
    }

    #endregion

    #region GetClass

    /// <summary>
    /// Returns the datatype class identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetClass
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns datatype class identifier if successful; otherwise
    /// <code>H5T_NO_CLASS</code>.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_class")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static partial int H5Tget_class(long type_id);

    public static H5Class GetClass(H5Type typeId)
    {
        return (H5Class)H5Tget_class(typeId);
    }

    #endregion

    public static H5Type CreateCompoundType(int size)
    {
        long h = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
        h.ThrowIfInvalidHandleValue("H5T.create");
        return new H5Type(h);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="S" />
    /// </summary>
    public static H5Type CreateCompoundType<S>() where S : struct
    {
        int size = Marshal.SizeOf<S>();
        return CreateCompoundType(size);
    }

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="S" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    public static H5Type CreateCompoundType<S>(int extraSpace) where S : struct
    {
        int size = Marshal.SizeOf<S>() + extraSpace;
        return CreateCompoundType(size);
    }

    public static H5Type CreateByteArrayType(int size)
    {
        long h = H5T.array_create(H5T.NATIVE_B8, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue("H5T.array_create");
        return new H5Type(h);
    }

    public static H5Type CreateDoubleArrayType(int size)
    {
        long h = H5T.array_create(H5T.NATIVE_DOUBLE, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue("H5T.array_create");
        return new H5Type(h);
    }

    public static H5Type CreateFloatArrayType(int size)
    {
        long h = H5T.array_create(H5T.NATIVE_FLOAT, 1, new[] { (ulong)size });
        h.ThrowIfInvalidHandleValue("H5T.array_create");
        return new H5Type(h);
    }

    public static H5Type CreateVariableLengthByteArrayType()
    {
        long h = H5T.vlen_create(H5T.NATIVE_B8);
        h.ThrowIfInvalidHandleValue("H5T.vlen_create");
        return new H5Type(h);
    }

    public static H5Type CreateFixedLengthStringType(int length)
    {
        long h = H5T.copy(H5T.C_S1);
        h.ThrowIfInvalidHandleValue("H5T.copy");
        int err = H5T.set_size(h, new IntPtr(length));
        err.ThrowIfError("H5T.set_size");
        return new H5Type(h);
    }

    public static void Insert(H5Type typeId, string name, IntPtr offset, long nativeTypeId)
    {
        int err = H5T.insert(typeId, name, offset, nativeTypeId);
        err.ThrowIfError("H5T.insert");
    }

    public static void Insert(H5Type typeId, string name, IntPtr offset, H5Type dataTypeId)
    {
        int err = H5T.insert(typeId, name, offset, dataTypeId);
        err.ThrowIfError("H5T.insert");
    }

    public static H5Type GetType(long handle)
    {
        handle.AssertIsHandleType(HandleType.DataSet, HandleType.Attribute);

        long typeHandle = H5A.get_type(handle);
        typeHandle.ThrowIfInvalidHandleValue("H5A.get_type");
        return new H5Type(typeHandle);
    }

    public static bool IsVariableLengthString(H5Type typeId)
    {
        int err = H5T.is_variable_str(typeId);
        err.ThrowIfError("H5T.is_variable_str");
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
    NClasses // ??
}
