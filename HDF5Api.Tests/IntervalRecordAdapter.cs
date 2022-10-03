using System.Runtime.InteropServices;

namespace HDF5Api.Tests;

/// <summary>
///     A type converter for <see cref="IntervalRecord" />.
/// </summary>
public sealed class IntervalRecordAdapter : H5TypeAdapter<IntervalRecord, IntervalRecordAdapter.SIntervalRecord>
{
    private IntervalRecordAdapter() { }

    protected override SIntervalRecord Convert(IntervalRecord source)
    {
        return new SIntervalRecord
        {
            Id = source.Id,
            Timestamp = source.Timestamp.ToOADate(),
            AverageThickness = source.AverageThickness ?? double.NaN,
            MinimumThickness = source.MinimumThickness ?? double.NaN,
            MaximumThickness = source.MaximumThickness ?? double.NaN
        };
    }

    public override H5Type GetH5Type()
    {
        return H5Type
            .CreateCompoundType<SIntervalRecord>()
            .Insert<SIntervalRecord, long>(nameof(SIntervalRecord.Id))
            .Insert<SIntervalRecord, double>(nameof(SIntervalRecord.Timestamp))
            .Insert<SIntervalRecord, double>(nameof(SIntervalRecord.AverageThickness))
            .Insert<SIntervalRecord, double>(nameof(SIntervalRecord.MinimumThickness))
            .Insert<SIntervalRecord, double>(nameof(SIntervalRecord.MaximumThickness));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIntervalRecord
    {
        public long Id;
        public double Timestamp;
        public double AverageThickness;
        public double MinimumThickness;
        public double MaximumThickness;
    }

    public static IH5TypeAdapter<IntervalRecord> Default { get; } = new IntervalRecordAdapter();
}

public class IntervalRecord
{
    public long Id { get; set; }

    public DateTime Timestamp { get; set; }

    public double? AverageThickness { get; set; }

    public double? MinimumThickness { get; set; }

    public double? MaximumThickness { get; set; }
}
