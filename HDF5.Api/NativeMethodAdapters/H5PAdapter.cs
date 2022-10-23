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
        int err = close(propertyList);

        err.ThrowIfError();
    }

    internal static TPList Create<TPList>(long classId, Func<long, TPList> createPropertyList) where TPList : H5PropertyList
    {
        long h = create(classId);

        h.ThrowIfInvalidHandleValue();

        return createPropertyList(h);
    }

    internal static long Create(long classId)
    {
        long h = create(classId);

        h.ThrowIfInvalidHandleValue();

        return h;
    }

    internal static void SetChunk(H5PropertyList propertyList, int rank, IEnumerable<long> dims)
    {
        int err = set_chunk(propertyList, rank, dims.Select(d => (ulong)d).ToArray());

        err.ThrowIfError();
    }

    internal static void SetDeflate(H5PropertyList propertyList, int level)
    {
        int err = set_deflate(propertyList, (uint)level);

        err.ThrowIfError();
    }

    internal static bool AreEqual(long propertyListId1, long propertyListId2)
    {
        int err = equal(propertyListId1, propertyListId2);

        err.ThrowIfError();

        return err != 0;
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
        long h = get_plist(obj);

        h.ThrowIfInvalidHandleValue();

        return createPropertyList(h);
    }

    internal static CharacterSet GetCharacterEncoding(H5PropertyList propertyList)
    {
        H5T.cset_t encoding = default;
        int err = get_char_encoding(propertyList, ref encoding);
        err.ThrowIfError();
        return (CharacterSet)encoding;
    }

    internal static void SetCharacterEncoding(H5PropertyList propertyList, CharacterSet encoding)
    {
        int err = set_char_encoding(propertyList, (H5T.cset_t)encoding);
        err.ThrowIfError();
    }

    internal static void SetCreateIntermediateGroups(H5PropertyList propertyList, bool value)
    {
        int err = set_create_intermediate_group(propertyList, value ? 1 : (uint)0);
        err.ThrowIfError();
    }

    internal static bool GetCreateIntermediateGroups(H5PropertyList propertyList)
    {
        uint value = 0;
        int err = get_create_intermediate_group(propertyList, ref value);
        err.ThrowIfError();
        return value != 0;
    }

    internal static long GetClassId(H5PropertyList propertyList)
    {
        long cls = get_class(propertyList);
        cls.ThrowIfError();
        return cls;
    }

    internal static void SetLibraryVersionBounds(H5PropertyList propertyList, LibraryVersion low, LibraryVersion high)
    {
        int retval = set_libver_bounds(propertyList, (H5F.libver_t)low, (H5F.libver_t)high);
        retval.ThrowIfError();
    }

    internal static (LibraryVersion low, LibraryVersion high) GetLibraryVersionBounds(H5PropertyList propertyList)
    {
        H5F.libver_t low = default;
        H5F.libver_t high = default;

        int retval = get_libver_bounds(propertyList, ref low, ref high);
        retval.ThrowIfError();

        return ((LibraryVersion)low, (LibraryVersion)high);
    }
}
