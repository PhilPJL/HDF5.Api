﻿using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static HDF5.Api.NativeMethods.H5T;
using HDF5.Api.Utils;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_t.html"/>
/// </summary>
internal static unsafe class H5TAdapter
{
    internal static bool AreEqual(H5Type type1, H5Type type2)
    {
        return equal(type1, type2).ThrowIfError() > 0;
    }

    internal static void Close(H5Type type)
    {
        close(type).ThrowIfError();
    }

    internal static DataTypeClass GetClass(H5Type type)
    {
        return (DataTypeClass)get_class(type);
    }

    internal static void SetCharacterSet(H5Type type, CharacterSet cset)
    {
        set_cset(type, (cset_t)cset).ThrowIfError();
    }

    internal static CharacterSet GetCharacterSet(H5Type type)
    {
        return (CharacterSet)((int)get_cset(type)).ThrowIfError();
    }

    internal static void SetUTF8(H5Type type) => SetCharacterSet(type, CharacterSet.Utf8);
    internal static void SetAscii(H5Type type) => SetCharacterSet(type, CharacterSet.Ascii);

    /// <summary>
    ///     Create a Compound type in order to hold an <typeparamref name="T" /> plus additional space as defined by
    ///     <paramref name="extraSpace" />
    /// </summary>
    internal static TT CreateCompoundType<T, TT>(Func<long, TT> typeCtor, int extraSpace = 0)
        where T : unmanaged
    {
        int size = sizeof(T) + extraSpace;
        return typeCtor(create((class_t)DataTypeClass.Compound, new ssize_t(size)));
    }

    internal static TT CreatePrimitiveArrayType<T, TT>(Func<long, TT> typeCtor, params long[] dims)
        where TT : H5Type
    {
        var handle = GetNativeTypeHandleForPrimitiveDotNetType<T>();

        return typeCtor(array_create(handle, (uint)dims.Length, dims.Select(d => (ulong)d).ToArray()));
    }

    private static long GetNativeTypeHandleForPrimitiveDotNetType<T>()
    {
        var type = typeof(T);

        if (!type.IsPrimitive)
        {
            throw new H5Exception($"{typeof(T).Name} is not primitive");
        }

        var handle = Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => NATIVE_HBOOL,

            TypeCode.Byte => NATIVE_UINT8,
            TypeCode.SByte => NATIVE_INT8,

            TypeCode.Int16 => NATIVE_INT16,
            TypeCode.UInt16 => NATIVE_UINT16,

            TypeCode.Int32 => NATIVE_INT32,
            TypeCode.UInt32 => NATIVE_UINT32,

            TypeCode.Int64 => NATIVE_INT64,
            TypeCode.UInt64 => NATIVE_UINT64,

            TypeCode.Single => NATIVE_FLOAT,
            TypeCode.Double => NATIVE_DOUBLE,

            // .NET char is 16 bit (UTF-16-ish)
            TypeCode.Char => NATIVE_UINT16,

            _ => throw new H5Exception($"No mapping defined from {typeof(T).Name} to native type.")
        };
        return handle;
    }

    internal static H5Type CreateByteArrayType(params long[] dims)
    {
        return new H5Type(array_create(NATIVE_B8, (uint)dims.Length, dims.Select(d => (ulong)d).ToArray()));
    }

    internal static H5Type CreateDoubleArrayType(int size)
    {
        return new H5Type(array_create(NATIVE_DOUBLE, 1, new[] { (ulong)size }));
    }

    internal static H5Type CreateFloatArrayType(int size)
    {
        return new H5Type(array_create(NATIVE_FLOAT, 1, new[] { (ulong)size }));
    }

    internal static H5Type CreateVariableLengthByteArrayType()
    {
        return new H5Type(vlen_create(NATIVE_B8));
    }

    internal static H5StringType CreateFixedLengthStringType(int storageLengthBytes)
    {
        var type = new H5StringType(copy(C_S1));

        try
        {
            type.Size = storageLengthBytes;
            return type;
        }
        catch
        {
            type.Dispose();
            throw;
        }
    }

    internal static H5StringType CreateVariableLengthStringType()
    {
        return new H5StringType(create(class_t.STRING, VARIABLE));
    }

    internal static TT CreateOpaqueType<TT>(int size, string tag, Func<long, TT> typeCtor)
        where TT : H5Type
    {
        var type = typeCtor(create(class_t.OPAQUE, new IntPtr(size)));

#if NET7_0_OR_GREATER
        set_tag(type, tag).ThrowIfError();
#else
        fixed (byte* tagBytesPtr = Encoding.UTF8.GetBytes(tag))
        {
            set_tag(type, tagBytesPtr).ThrowIfError();
        }
#endif

        return type;
    }

    internal static void InsertEnumMember<T>(H5Type type, string name, T value)
    //where T : Enum //unmanaged, 
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#if NET7_0_OR_GREATER
        enum_insert(type, name, new IntPtr(&value)).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            enum_insert(type, nameBytesPtr, new IntPtr(&value)).ThrowIfError();
        }
#endif
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    internal static string NameOfEnumMember<T>(H5Type type, T value)
        where T : unmanaged, Enum
    {
        const int length = 512;

#if NET7_0_OR_GREATER
        using var bufferOwner = SpanOwner<byte>.Allocate(length);
        var buffer = bufferOwner.Span;
        enum_nameof(type, new nint(&value), buffer, length).ThrowIfError();
        int nullTerminatorIndex = MemoryExtensions.IndexOf(buffer, (byte)0);
        nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
        return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
#else
        var buffer = new byte[length];
        fixed (byte* bufferPtr = buffer)
        {
            enum_nameof(type, new IntPtr(&value), bufferPtr, new IntPtr(length)).ThrowIfError();
            Span<byte> bytes = buffer;
            // ReSharper disable once InvokeAsExtensionMethod
            var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
            nullTerminatorIndex = nullTerminatorIndex < 0 ? length : nullTerminatorIndex;
            return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
        }
#endif
    }

    internal static T ValueOfEnumMember<T>(H5Type type, string name)
        where T : unmanaged, Enum
    {
        T value = default;

#if NET7_0_OR_GREATER
        enum_valueof(type, name, new nint(&value)).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            enum_valueof(type, nameBytesPtr, new IntPtr(&value)).ThrowIfError();
        }
#endif

        return value;
    }

    internal static void Insert(H5Type type, string name, ssize_t offset, long nativeTypeId)
    {
#if NET7_0_OR_GREATER
        insert(type, name, offset, nativeTypeId).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            insert(type, nameBytesPtr, offset, nativeTypeId).ThrowIfError();
        }
#endif
    }

    internal static void Insert(H5Type type, string name, ssize_t offset, H5Type dataTypeId)
    {
        Insert(type, name, offset, (long)dataTypeId);
    }

    internal static bool IsVariableLengthString(H5Type typeId)
    {
        return is_variable_str(typeId).ThrowIfError() > 0;
    }

    internal static string GetMemberName(H5Type type, int index)
    {
        var name = get_member_name(type, (uint)index);

        if (name == IntPtr.Zero)
        {
            throw new H5Exception("Unable to retrieve member name.");
        }

        try
        {
            return MarshalHelpers.PtrToStringUTF8(name);
        }
        finally
        {
            H5.free_memory(name);
        }
    }

    internal static int GetNumberOfMembers(H5Type type)
    {
        return get_nmembers(type).ThrowIfError();
    }

    internal static IEnumerable<string> GetMemberNames(H5Type type)
    {
        int count = GetNumberOfMembers(type);
        int index = 0;
        while (index < count)
        {
            yield return GetMemberName(type, index);
            index++;
        }
    }

    internal static TT CreateEnumType<T, TT>(Func<long, TT> typeCtor) //where T : Enum // unmanaged, 
        where TT : H5Type
    {
        H5ThrowHelpers.ThrowIfNotEnum<T>();

        var h5EnumType = ConvertDotNetEnumUnderlyingTypeToH5NativeType<T, TT>(typeCtor);

        try
        {
            foreach (var enumInfo in typeof(T)
                .GetMembers(BindingFlags.Public | BindingFlags.Static)
                .Select(m => new
                {
                    m.Name,
                    Value = (T)Enum.Parse(typeof(T), m.Name)
                }))
            {
                InsertEnumMember(h5EnumType, enumInfo.Name, enumInfo.Value);
            }
        }
        catch
        {
            h5EnumType.Dispose();
            throw;
        }

        return h5EnumType;
    }

    internal static H5PrimitiveType<T> ConvertDotNetEnumUnderlyingTypeToH5PrimitiveType<T>() // where T : unmanaged, Enum
    {
        return ConvertDotNetEnumUnderlyingTypeToH5NativeType<T, H5PrimitiveType<T>>(h => new H5PrimitiveType<T>(h));
    }

    internal static TT ConvertDotNetEnumUnderlyingTypeToH5NativeType<T, TT>(Func<long, TT> typeCtor) // where T : unmanaged, Enum
        where TT : H5Type
    {
        var baseType = GetNativeType();

        return typeCtor(enum_create(baseType));

        static long GetNativeType()
        {
            var underlyingType = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));

            return underlyingType switch
            {
                TypeCode.Byte => NATIVE_UINT8,
                TypeCode.SByte => NATIVE_INT8,

                TypeCode.Int16 => NATIVE_INT16,
                TypeCode.UInt16 => NATIVE_UINT16,

                TypeCode.Int32 => NATIVE_INT32,
                TypeCode.UInt32 => NATIVE_UINT32,

                TypeCode.Int64 => NATIVE_INT64,
                TypeCode.UInt64 => NATIVE_UINT64,

                _ => throw new ArgumentException($"Unable to create Enum for underlying type '{underlyingType}'.")
            };
        }
    }

    /// <summary>
    /// Gets the equivalent native type for a primitive .NET type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="H5Exception"></exception>
    internal static H5PrimitiveType<T> ConvertDotNetPrimitiveToH5NativeType<T>() // where T : unmanaged /* primitive would be nice */
    {
        return ConvertDotNetPrimitiveToH5Type<T, H5PrimitiveType<T>>(h => new H5PrimitiveType<T>(h));
    }

    internal static TT ConvertDotNetPrimitiveToH5Type<T, TT>(Func<long, TT> typeCtor)
        where TT : H5Type
    {
        var handle = GetNativeTypeHandleForPrimitiveDotNetType<T>();

        // Copy the static handle so we can track and close it
        return typeCtor(copy(handle));
    }

    internal static H5Type GetEquivalentNativeType(H5Type type)
    {
        return new H5Type(get_native_type(type, direction_t.DEFAULT));
    }

    internal static void SetPadding(H5Type h5Type, StringPadding padding)
    {
        set_strpad(h5Type, (str_t)padding).ThrowIfError();
    }

    internal static StringPadding GetPadding(H5Type h5Type)
    {
        return (StringPadding)((int)get_strpad(h5Type)).ThrowIfError();
    }

    internal static void SetSize(H5Type h5Type, int size)
    {
        set_size(h5Type, new IntPtr(size)).ThrowIfError();
    }

    internal static int GetSize(H5Type h5Type)
    {
        return ((int)get_size(h5Type)).ThrowIfError();
    }

    internal static bool GetCommitted(H5Type h5Type)
    {
        return committed(h5Type).ThrowIfError() > 0;
    }

    internal static void Commit<T>(
        [DisallowNull] H5Object<T> h5Object,
        [DisallowNull] string name,
        [DisallowNull] H5Type h5Type,
        [AllowNull] H5DataTypeCreationPropertyList? dataTypeCreationPropertyList,
        [AllowNull] H5DataTypeAccessPropertyList? dataTypeAccessPropertyList) where T : H5Object<T>
    {
        h5Object.AssertHasLocationHandleType();

        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);
        Guard.IsNotNull(h5Type);

        using var linkCreationPropertyList = H5Link.CreateCreationPropertyList();

#if NET7_0_OR_GREATER
        commit(h5Object, name, h5Type,
            linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList)
            .ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            commit(h5Object, nameBytesPtr, h5Type,
                linkCreationPropertyList, dataTypeCreationPropertyList, dataTypeAccessPropertyList)
                .ThrowIfError();
        }
#endif
    }

    internal static H5DataTypeCreationPropertyList CreateCreationPropertyList()
    {
        return H5PAdapter.Create(H5P.DATATYPE_CREATE, h => new H5DataTypeCreationPropertyList(h));
    }

    internal static H5DataTypeAccessPropertyList CreateAccessPropertyList()
    {
        return H5PAdapter.Create(H5P.DATATYPE_ACCESS, h => new H5DataTypeAccessPropertyList(h));
    }

    internal static void Lock(H5Type type)
    {
        lock_datatype(type).ThrowIfError();
    }

    internal static H5Type Open<T>(
        [DisallowNull] H5Object<T> h5Object,
        [DisallowNull] string name,
        [AllowNull] H5DataTypeAccessPropertyList? dataTypeAccessPropertyList) where T : H5Object<T>
    {
        h5Object.AssertHasLocationHandleType();

        Guard.IsNotNull(h5Object);
        Guard.IsNotNullOrEmpty(name);

        long h;

#if NET7_0_OR_GREATER
        h = open(h5Object, name, dataTypeAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(h5Object, nameBytesPtr, dataTypeAccessPropertyList);
        }
#endif

        return new H5Type(h);
    }
}
