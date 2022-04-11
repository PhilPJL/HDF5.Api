using HDF.PInvoke;
using HDF5Api;
using HDF5Api.Disposables;
using PulseData.TvlAlt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="Profile"/>.
    /// </summary>
    public sealed class ProfileAdapter : H5TypeAdapter<Profile>
    {
        private readonly int ProfileBlobSize;
        private readonly int ProfileBlobTypeSize;
        private const int UnitsLength = 6;

        private ProfileAdapter(int profileBlobSize, int profileBlobTypeSize)
        {
            AssertBlobLengthDivisibleByTypeLength(profileBlobSize, profileBlobTypeSize);

            ProfileBlobSize = profileBlobSize;
            ProfileBlobTypeSize = profileBlobTypeSize;
        }

        public override H5Type GetH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(ProfileBlobSize / ProfileBlobTypeSize / 2);
            using var stringType = H5Type.CreateFixedLengthStringType(UnitsLength);

            int xValuesOffset = Marshal.SizeOf<SProfile>();
            int zValuesOffset = xValuesOffset + ProfileBlobSize / 2;

            return H5Type
                .CreateCompoundType<SProfile>(ProfileBlobSize)
                .Insert<SProfile>(nameof(SProfile.Id), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.RecordId), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.Units), stringType)
                .Insert("XValues", xValuesOffset, valuesType)
                .Insert("ZValuex", zValuesOffset, valuesType);
        }

        public override void WriteChunk(Action<IntPtr> write, IEnumerable<Profile> inputRecords)
        {
            static unsafe SProfile Convert(Profile source)
            {
                var result = new SProfile()
                {
                    Id = source.Id,
                    RecordId = source.RecordId
                };

                CopyString(source.Units, result.Units, UnitsLength);

                return result;
            }

            var records = inputRecords.Select(Convert).ToList();
            var blobs = inputRecords.Select(r => new { r.Id, r.Values }).ToList();

            // if any record.Value.Length != CorrectionBlobSize, throw
            var invalidLengths = blobs.Where(r => r.Values.Length != ProfileBlobSize).ToList();
            if (invalidLengths.Any())
            {
                throw new InvalidOperationException($"The profile values length in Profile for ids '{string.Join(",", invalidLengths.Select(l => l.Id))}' does not match the expected length of {ProfileBlobSize}.");
            }

            int structSize = Marshal.SizeOf<SProfile>();
            int bufferSize = (structSize + ProfileBlobSize) * records.Count;

            using var buffer = new GlobalMemory(bufferSize);

            unsafe
            {
                var remainingBufferSize = bufferSize;

                for (int i = 0; i < records.Count; i++)
                {
                    using var pinnedRecord = new PinnedObject(records[i]);
                    using var pinnedBlob = new PinnedObject(blobs[i].Values);

                    if (records[i].Id != blobs[i].Id)
                    {
                        throw new InvalidOperationException($"Profile converter. Unexpected internal error. Ids not matching.");
                    }

                    // Copy SProfile
                    Buffer.MemoryCopy(pinnedRecord, CurrentBufferPosition(), remainingBufferSize, structSize);
                    remainingBufferSize -= structSize;

                    // Copy values
                    Buffer.MemoryCopy(pinnedBlob, CurrentBufferPosition(), remainingBufferSize, ProfileBlobSize);
                    remainingBufferSize -= ProfileBlobSize;
                }

                void* CurrentBufferPosition()
                {
                    return IntPtr.Add(buffer.IntPtr, bufferSize - remainingBufferSize).ToPointer();
                }
            }

            write(buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SProfile
        {
            public long Id;
            public long RecordId;

            public fixed byte Units[UnitsLength];
        }

        public static IH5TypeAdapter<Profile> Create(int profileBlobSize, int profileBlobTypeSize) => new ProfileAdapter(profileBlobSize, profileBlobTypeSize);
    }
}
