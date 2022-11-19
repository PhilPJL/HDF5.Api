using System.Runtime.InteropServices;
using HDF5.Api.H5Types;

namespace HDF5.Api.Tests;

/// <summary>
///     A type converter for <see cref="TestRecord" />.
/// </summary>
internal sealed class TestRecordAdapter : H5TypeAdapter<TestRecord, TestRecordAdapter.STestRecord>
{
    private TestRecordAdapter() { }

    protected override STestRecord Convert(TestRecord source)
    {
        return new STestRecord
        {
            Id = source.Id,
            ShortProperty = source.ShortProperty,
            IntProperty = source.IntProperty,
            LongProperty = source.LongProperty,
            UShortProperty = source.UShortProperty,
            UIntProperty = source.UIntProperty,
            ULongProperty = source.ULongProperty,
            FloatProperty = source.FloatProperty,
            DoubleProperty = source.DoubleProperty
        };
    }

    public override H5Type GetH5Type()
    {
        return H5Type
            .CreateCompoundType<STestRecord, H5CompoundType<STestRecord>>(h => new H5CompoundType<STestRecord>(h))
            .Insert<STestRecord, int>(nameof(STestRecord.Id))
            .Insert<STestRecord, short>(nameof(STestRecord.ShortProperty))
            .Insert<STestRecord, int>(nameof(STestRecord.IntProperty))
            .Insert<STestRecord, long>(nameof(STestRecord.LongProperty))
            .Insert<STestRecord, ushort>(nameof(STestRecord.UShortProperty))
            .Insert<STestRecord, uint>(nameof(STestRecord.UIntProperty))
            .Insert<STestRecord, ulong>(nameof(STestRecord.ULongProperty))
            .Insert<STestRecord, float>(nameof(STestRecord.FloatProperty))
            .Insert<STestRecord, double>(nameof(STestRecord.DoubleProperty))
            ;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STestRecord
    {
        public int Id;

        public short ShortProperty;
        public int IntProperty;
        public long LongProperty;
        public ushort UShortProperty;
        public uint UIntProperty;
        public ulong ULongProperty;
        public float FloatProperty;
        public double DoubleProperty;
    }

    public static IH5TypeAdapter<TestRecord> Default { get; } = new TestRecordAdapter();
}

public class TestRecord
{
    public int Id { get; set; }
    public short ShortProperty { get; set; }
    public int IntProperty { get; set; }
    public long LongProperty { get; set; }
    public ushort UShortProperty { get; set; }
    public uint UIntProperty { get; set; }
    public ulong ULongProperty { get; set; }
    public float FloatProperty { get; set; }
    public double DoubleProperty { get; set; }
}
