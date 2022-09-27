using System;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5 (Global) API.
/// </summary>
public static class H5Global
{
    public static Version GetLibraryVersion()
    {
        uint major = 0;
        uint minor = 0;
        uint revision = 0;

        int err = H5.get_libversion(ref major, ref minor, ref revision);

        err.ThrowIfError("H5.get_libversion");

        return new Version((int)major, (int)minor, 0, (int)revision);
    }

    /// <summary>
    ///     Is the referenced library built with thread-safe option?
    /// </summary>
    public static bool IsThreadSafe()
    {
        uint is_ts = 0;
        int err = H5.is_library_threadsafe(ref is_ts);

        err.ThrowIfError("H5.is_library_threadsafe");

        return is_ts != 0;
    }
}

/// <summary>
/// Common iteration orders.  Equivalent to H5.iter_order_t.
/// </summary>
internal enum CommonIterationOrders : int
{
    /// <summary>
    /// Unknown order [value = -1].
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// Increasing order [value = 0].
    /// </summary>
    Increasing,
    /// <summary>
    /// Decreasing order [value = 1].
    /// </summary>
    Decreasing,
    /// <summary>
    /// No particular order, whatever is fastest [value = 2].
    /// </summary>
    Native,
    /// <summary>
    /// Number of iteration orders [value = 3].
    /// </summary>
    N
}

/// <summary>
/// The types of indices on links in groups/attributes on objects.
/// Primarily used for "[do] [foo] by index" routines and for iterating
/// over links in groups/attributes on objects.
/// Equivalent to H5.index_t.
/// </summary>
internal enum GroupOrAttributeIndex : int
{
    /// <summary>
    /// Unknown index type [value = -1].
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// Index on names [value = 0].
    /// </summary>
    Name,
    /// <summary>
    /// Index on creation order [value = 1].
    /// </summary>
    CreationIndex,
    /// <summary>
    /// Number of indices defined [value = 2].
    /// </summary>
    N
};

/*internal static class Constants
{
    public const string DLLFileName = "hdf5.dll";
};*/