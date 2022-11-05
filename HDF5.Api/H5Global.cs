using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static HDF5.Api.NativeMethods.H5;
using static HDF5.Api.NativeMethods.H5T;

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

        int result = get_libversion(ref major, ref minor, ref revision);

        result.ThrowIfError();

        return new Version((int)major, (int)minor, 0, (int)revision);
    }

    /// <summary>
    ///     Is the referenced library built with thread-safe option?
    /// </summary>
    public static bool IsThreadSafe()
    {
        uint is_ts = 0;
        int result = is_library_threadsafe(ref is_ts);

        result.ThrowIfError();

        return is_ts != 0;
    }

    /// <summary>
    /// It's not possible to determine the size of buffers allocated (internally by HDF) when retrieving
    /// variable length strings.  As a safety measure assume variable length strings are never longer than 
    /// this.
    /// </summary>
    public static int MaxVariableLengthStringBuffer { get; set; } = 0x100000;

    public static AttributeWriteBehaviour DefaultAttributeWriteBehaviour { get; set; } = AttributeWriteBehaviour.Default;
}

[Flags]
public enum FileObjectType : uint
{
    All = H5F.OBJ_ALL,
    Attribute = H5F.OBJ_ATTR,
    DataSet = H5F.OBJ_DATASET,
    DataType = H5F.OBJ_DATATYPE,
    File = H5F.OBJ_FILE,
    Group = H5F.OBJ_GROUP,
    Local = H5F.OBJ_LOCAL
}

public enum ObjectType
{
    Group = H5O.type_t.GROUP,
    DataSet = H5O.type_t.DATASET,
    NamedDataType = H5O.type_t.NAMED_DATATYPE
}

public readonly struct Dimension
{
    public const ulong Unlimited = ulong.MaxValue;

    public ulong InitialSize { get; }
    public ulong UpperLimit { get; }

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

    public static Dimension[] Create(params long[] dimensions)
    {
        return dimensions.Select(d => new Dimension(d)).ToArray();
    }

    public static Dimension[] Create(params (long initialSize, long upperLimit)[] dimensions)
    {
        return dimensions.Select(d => new Dimension(d.initialSize, d.upperLimit)).ToArray();
    }
}

public enum DataTypeClass
{
    None = class_t.NO_CLASS,
    Integer = class_t.INTEGER,
    Float = class_t.FLOAT,
    Time = class_t.TIME,
    String = class_t.STRING,
    BitField = class_t.BITFIELD,
    Opaque = class_t.OPAQUE,
    Compound = class_t.COMPOUND,
    Reference = class_t.REFERENCE,
    Enum = class_t.ENUM,
    VariableLength = class_t.VLEN,
    Array = class_t.ARRAY,
    NClasses
}

public enum PropertyListClass
{
    None = 0,
    ObjectCreate = 1,
    FileCreate = 2,
    FileAccess = 3,
    DatasetCreate = 4,
    DatasetAccess = 5,
    DatasetXfer = 6,
    FileMount = 7,
    GroupCreate = 8,
    GroupAccess = 9,
    DataTypeCreate = 10,
    DataTypeAccess = 11,
    StringCreate = 12,
    LinkCreate = 13,
    LinkAccess = 14
}

public enum StringPadding
{
    Space = str_t.SPACEPAD,
    NullPad = str_t.NULLPAD,
    NullTerminate = str_t.NULLTERM
}

public enum CharacterSet
{
    Ascii = cset_t.ASCII,
    Utf8 = cset_t.UTF8
}

public enum LibraryVersion
{
    /// <summary>
    /// Use the earliest possible format for storing objects
    /// </summary>
    Earliest = H5F.libver_t.EARLIEST,
    /// <summary>
    /// Use the latest v18 format for storing objects
    /// </summary>
    Version18 = H5F.libver_t.V18,
    /// <summary>
    /// Use the latest v110 format for storing objects
    /// </summary>
    Version110 = H5F.libver_t.V110,
    /// <summary>
    /// Use the latest possible format for storing objects
    /// </summary>
    Latest = Version110
}

/// <summary>
/// Specify behaviour when writing a value to an attribute which already exists
/// </summary>
public enum AttributeWriteBehaviour
{
    /// <summary>
    /// Global default = CreateOrUpdate
    /// </summary>
    Default = CreateOrUpdate,

    /// <summary>
    /// If the attribute does not exist either then create.
    /// If the attribute does exist, check it's the correct type and then update the value.
    /// </summary>
    CreateOrUpdate = 0,

    /// <summary>
    /// Delete the existing attribute, create new attribute and set value.
    /// </summary>
    OverwriteIfAlreadyExists = 1,

    /// <summary>
    /// Throw exception if already exists.
    /// </summary>
    ThrowIfAlreadyExists = 2
}

public enum DeleteAttributeStatus
{
    NotFound,
    Deleted
}