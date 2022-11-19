using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;
internal class H5StringType : H5Type<string>
{
    internal H5StringType(long handle) : base(handle)
    {
    }

    internal static H5StringType Create(int fixedStorageLength,
        CharacterSet characterSet = CharacterSet.Utf8,
        StringPadding padding = StringPadding.NullPad)
    {
        var type = fixedStorageLength != 0
            ? H5TAdapter.CreateFixedLengthStringType(fixedStorageLength)
            : H5TAdapter.CreateVariableLengthStringType();

        type.CharacterSet = characterSet;
        type.StringPadding = padding;

        return type;
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