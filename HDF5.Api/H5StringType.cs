using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

public class H5StringType : H5Type
{
    internal H5StringType(long handle) : base(handle)
    {
    }

    public static H5Type CreateFixedLengthStringType(int length)
    {
        return H5TAdapter.CreateFixedLengthStringType(length);
    }

    public static H5StringType CreateVariableLengthStringType()
    {
        return H5TAdapter.CreateVariableLengthStringType();
    }
}