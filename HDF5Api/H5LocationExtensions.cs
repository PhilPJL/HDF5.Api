using HDF5Api.Disposables;
using System;

namespace HDF5Api
{
    public static class H5LocationExtensions
    {
        private static void CreateAndWriteAttribute(IH5Location location, string name, H5TypeHandle typeId, IntPtr buffer)
        {
            // Single dimension (rank 1), unlimited length, chunk size.
            using (var memorySpace = H5Space.CreateSimple(1, new ulong[] { 1 }, new ulong[] { 1 }))

            // Create a attribute-creation property list
            using (var properyList = H5PropertyList.Create(H5P.ATTRIBUTE_CREATE))

            // Create a dataset with our record type and chunk size.
            using (var attribute = location.CreateAttribute(name, typeId, memorySpace, properyList))
            {
                attribute.Write(typeId, buffer);
            }
        }

        public static void CreateAndWriteAttribute(this IH5Location location, string name, DateTime value)
        {
            CreateAndWriteAttribute(location, name, value.ToOADate());
        }

        public static void CreateAndWriteAttribute(this IH5Location location, string name, int value)
        {
            using (var typeId = H5TypeHandle.WrapNative(H5T.NATIVE_INT32))
            using (var pinned = new PinnedObject(value))
            {
                CreateAndWriteAttribute(location, name, typeId, pinned);
            }
        }

        public static void CreateAndWriteAttribute(this IH5Location location, string name, long value)
        {
            using (var typeId = H5TypeHandle.WrapNative(H5T.NATIVE_INT64))
            using (var pinned = new PinnedObject(value))
            {
                CreateAndWriteAttribute(location, name, typeId, pinned);
            }
        }

        public static void CreateAndWriteAttribute(this IH5Location location, string name, double value)
        {
            using (var typeId = H5TypeHandle.WrapNative(H5T.NATIVE_DOUBLE))
            using (var pinned = new PinnedObject(value))
            {
                CreateAndWriteAttribute(location, name, typeId, pinned);
            }
        }

        public static void CreateAndWriteAttribute(this IH5Location location, string name, string value, int maxLength = 0)
        {
            value = value ?? string.Empty;

            if (maxLength <= 0)
            {
                maxLength = value.Length;
            }
            else
            {
                maxLength = Math.Min(value.Length, maxLength);
            }

            var subString = (value.Length > maxLength) ? value.Substring(0, maxLength) : value;

            // can't create a zero length string type so use a length of 1 minimum
            using (var typeId = H5Type.CreateFixedLengthStringType(subString.Length < 1 ? 1 : subString.Length))
            {
                byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);

                using (var pinned = new PinnedObject(sourceBytes))
                {
                    CreateAndWriteAttribute(location, name, typeId, pinned);
                }
            }
        }
    }
}
