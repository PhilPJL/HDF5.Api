
namespace HDF5.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class H5MemberAttribute : Attribute
    {
        public H5MemberAttribute(string? name = null) { Name = name ?? string.Empty; }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class H5StringMemberAttribute : H5MemberAttribute
    {
        public H5StringMemberAttribute(
            CharacterSet characterSet = CharacterSet.Ascii,
            StringPadding stringPadding = StringPadding.NullPad,
            int allocatedStorageInBytes = 0)
        {
            CharacterSet = characterSet;
            StringPadding = stringPadding;
            AllocatedStorageInBytes = allocatedStorageInBytes;
        }

        public CharacterSet CharacterSet { get; }
        public StringPadding StringPadding { get; }
        public int AllocatedStorageInBytes { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false)]
    public sealed class H5ContractAttribute : Attribute
    {
        public H5ContractAttribute(string? name = null) { Name = name ?? string.Empty; }
        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class H5IgnoreAttribute : Attribute { }


    /*    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
        public class H5Dimensions : Attribute
        {
            public H5Dimensions(params long[] dimensions)
            {
                Dimensions = Dimension.Create(dimensions);
            }

            public H5Dimensions(params (long initialSize, long upperLimit)[] dimensions)
            {
                Dimensions = Dimension.Create(dimensions);
            }

            public IEnumerable<Dimension> Dimensions { get; }
        }*/
}