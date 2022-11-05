using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5StringType : H5Type<string>
{
    internal H5StringType(long handle) : base(handle)
    {
    }

    internal static H5StringType CreateFixedLengthStringType(int length)
    {
        return H5TAdapter.CreateFixedLengthStringType(length);
    }

    internal static H5StringType CreateVariableLengthStringType()
    {
        return H5TAdapter.CreateVariableLengthStringType();
    }

    public CharacterSet CharacterSet
    {
        get => H5TAdapter.GetCharacterSet(this);
        internal set => H5TAdapter.SetCharacterSet(this, value);
    }

    public StringPadding StringPadding
    {
        get => H5TAdapter.GetPadding(this);
        internal set => H5TAdapter.SetPadding(this, value);
    }

    public bool IsVariableLength => H5TAdapter.IsVariableLengthString(this);
}