using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

public class H5StringType : H5Type<string> 
{
    internal H5StringType(long handle) : base(handle)
    {
    }

    public static H5StringType CreateFixedLengthStringType(int length)
    {
        return H5TAdapter.CreateFixedLengthStringType(length);
    }

    public static H5StringType CreateVariableLengthStringType()
    {
        return H5TAdapter.CreateVariableLengthStringType();
    }

    internal CharacterSet CharacterSet
    {
        get => H5TAdapter.GetCharacterSet(this);
        set => H5TAdapter.SetCharacterSet(this, value);
    }

    internal StringPadding StringPadding
    {
        get => H5TAdapter.GetPadding(this);
        set => H5TAdapter.SetPadding(this, value);
    }

    public bool IsVariableLength()
    {
        return H5TAdapter.IsVariableLengthString(this);
    }
}