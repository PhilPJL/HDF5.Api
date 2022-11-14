using System.Collections.Generic;
using HDF5.Api.NativeMethods;
using System.Linq;
using static HDF5.Api.NativeMethods.H5P;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
internal static class H5PAdapter
{
    internal static void Close(H5PropertyList propertyList)
    {
        close(propertyList).ThrowIfError();
    }

    internal static TPList Create<TPList>(long classId, Func<long, TPList> createPropertyList) where TPList : H5PropertyList
    {
        return createPropertyList(create(classId));
    }

    /// <summary>
    /// Create an object with a given class.  Doesn't require wrapping in an H5Object.
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    internal static long Create(long classId)
    {
        return create(classId);
    }

    internal static void SetChunk(H5PropertyList propertyList, int rank, IEnumerable<long> dims)
    {
        set_chunk(propertyList, rank, dims.Select(d => (ulong)d).ToArray()).ThrowIfError();
    }

    internal static void SetDeflate(H5PropertyList propertyList, int level)
    {
        set_deflate(propertyList, (uint)level).ThrowIfError();
    }

    internal static bool AreEqual(long propertyListId1, long propertyListId2)
    {
        return equal(propertyListId1, propertyListId2).ThrowIfError() != 0;
    }

    internal static bool AreEqual(H5PropertyList propertyList1, H5PropertyList propertyList2)
    {
        return AreEqual((long)propertyList1, (long)propertyList2);
    }

    internal static TPList GetPropertyList<T, TPList>(
            H5Object<T> obj, Func<long, long> get_plist, Func<long, TPList> createPropertyList)
        where T : H5Object<T>
        where TPList : H5PropertyList
    {
        return createPropertyList(get_plist(obj));
    }

    internal static CharacterSet GetCharacterEncoding(H5PropertyList propertyList)
    {
        H5T.cset_t encoding = default;
        get_char_encoding(propertyList, ref encoding).ThrowIfError();
        return (CharacterSet)encoding;
    }

    internal static void SetCharacterEncoding(H5PropertyList propertyList, CharacterSet encoding)
    {
        set_char_encoding(propertyList, (H5T.cset_t)encoding).ThrowIfError();
    }

    internal static void SetCreateIntermediateGroups(H5PropertyList propertyList, bool value)
    {
        set_create_intermediate_group(propertyList, value ? 1 : (uint)0).ThrowIfError();
    }

    internal static void SetAttributePhaseChange(H5PropertyList propertyList, int maxCompact, int minDense)
    {
        H5P.set_attr_phase_change(propertyList, (uint)maxCompact, (uint)minDense).ThrowIfError();
    }

    internal static bool GetCreateIntermediateGroups(H5PropertyList propertyList)
    {
        uint value = 0;
        get_create_intermediate_group(propertyList, ref value).ThrowIfError();
        return value != 0;
    }

    internal static long GetClassId(H5PropertyList propertyList)
    {
        return get_class(propertyList).ThrowIfError();
    }

    internal static void SetLibraryVersionBounds(H5PropertyList propertyList, LibraryVersion low, LibraryVersion high)
    {
        set_libver_bounds(propertyList, (H5F.libver_t)low, (H5F.libver_t)high).ThrowIfError();
    }

    internal static (LibraryVersion low, LibraryVersion high) GetLibraryVersionBounds(H5PropertyList propertyList)
    {
        H5F.libver_t low = default;
        H5F.libver_t high = default;

        get_libver_bounds(propertyList, ref low, ref high).ThrowIfError();

        return ((LibraryVersion)low, (LibraryVersion)high);
    }
}
