﻿using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Reflection;
using static HDF5.Api.NativeMethods.H5;

namespace HDF5.Api;

/// <summary>
///     Wrapper for H5 (Global) API.
/// </summary>
public static class H5Global
{
    public static (bool hdfLoaded, bool hdfHlLoaded) TryLoadLibraries([AllowNull] string? path = null)
    {
        bool hdfLoaded = NativeProviderLoader.TryLoad(Constants.DLLFileName, path ?? string.Empty);
        bool hdfHlLoaded = NativeProviderLoader.TryLoad(Constants.HLDLLFileName, path ?? string.Empty);
        return (hdfLoaded, hdfHlLoaded);
    }

    public static IEnumerable<(string name, string path)> LoadedLibraries => NativeProviderLoader.LoadedLibraries;

    public static string Describe()
    {
        var versionAttribute = typeof(H5Global).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;

        var sb = new StringBuilder();
        sb.AppendLine("HDF.API configuration:");
        sb.AppendLine($"Version {versionAttribute?.InformationalVersion}");
#if NET7_0
        sb.AppendLine("Built for .NET 7.0");
#elif NETSTANDARD2_0
        sb.AppendLine("Built for .NET Standard 2.0");
#endif

        sb.AppendLine($"Operating System: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"Operating System Architecture: {RuntimeInformation.OSArchitecture}");
        sb.AppendLine($"Framework: {RuntimeInformation.FrameworkDescription}");
        sb.AppendLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");

        sb.AppendLine($"HDF5 version: {GetLibraryVersion()}");
        sb.AppendLine($"HDF5 is thread safe: {IsThreadSafe()}");

        var processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        if (!string.IsNullOrEmpty(processorArchitecture))
        {
            sb.AppendLine($"Processor Architecture: {processorArchitecture}");
        }
        var processorId = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
        if (!string.IsNullOrEmpty(processorId))
        {
            sb.AppendLine($"Processor Identifier: {processorId}");
        }

        return sb.ToString();
    }

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
public enum H5FObjectType : uint
{
    All = H5F.OBJ_ALL,
    Attribute = H5F.OBJ_ATTR,
    DataSet = H5F.OBJ_DATASET,
    DataType = H5F.OBJ_DATATYPE,
    File = H5F.OBJ_FILE,
    Group = H5F.OBJ_GROUP,
    Local = H5F.OBJ_LOCAL
}

public enum H5ObjectType
{
    Group = H5O.type_t.GROUP,
    DataSet = H5O.type_t.DATASET,
    NamedDataType = H5O.type_t.NAMED_DATATYPE
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

public enum StringPadding
{
    Space = H5T.str_t.SPACEPAD,
    NullPad = H5T.str_t.NULLPAD,
    NullTerminate = H5T.str_t.NULLTERM
}

public enum CharacterSet
{
    Ascii = H5T.cset_t.ASCII,
    Utf8 = H5T.cset_t.UTF8
}

[Obsolete]
public enum PropertyListType
{
    None = 0,
    Create,
    Access
}
