using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethods;
using static HDF5.Api.NativeMethods.H5;

namespace HDF5.Api;

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

        int err = get_libversion(ref major, ref minor, ref revision);

        err.ThrowIfError();

        return new Version((int)major, (int)minor, 0, (int)revision);
    }

    /// <summary>
    ///     Is the referenced library built with thread-safe option?
    /// </summary>
    public static bool IsThreadSafe()
    {
        uint is_ts = 0;
        int err = is_library_threadsafe(ref is_ts);

        err.ThrowIfError();

        return is_ts != 0;
    }
}

[Flags]
public enum H5ObjectTypes : uint
{
    All = H5F.OBJ_ALL,
    Attribute = H5F.OBJ_ATTR,
    DataSet = H5F.OBJ_DATASET,
    DataType = H5F.OBJ_DATATYPE,
    File = H5F.OBJ_FILE,
    Group = H5F.OBJ_GROUP,
    Local = H5F.OBJ_LOCAL
}

public readonly struct Dimension
{
    public const ulong Unlimited = ulong.MaxValue;

    public readonly ulong InitialSize { get; }
    public readonly ulong UpperLimit { get; }

    public Dimension(long initialSize, long? upperLimit = null)
    {
        Guard.IsGreaterThanOrEqualTo(initialSize, 0);

        if (upperLimit.HasValue)
        {
            Guard.IsGreaterThanOrEqualTo(upperLimit.Value, initialSize);
        }

        InitialSize = (ulong)initialSize;

        if (upperLimit == null)
        {
            UpperLimit = Unlimited;
        }
        else
        {
            UpperLimit = (ulong)upperLimit.Value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="initialSize"></param>
    /// <param name="upperLimit">If null then no upper limit.</param>
    public Dimension(ulong initialSize, ulong? upperLimit = null)
    {
        InitialSize = initialSize;

        UpperLimit = upperLimit ?? Unlimited;
    }
};

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
