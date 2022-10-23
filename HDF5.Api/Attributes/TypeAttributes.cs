using System.Collections.Generic;

namespace HDF5.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class H5MemberAttribute : Attribute
    {
        public H5MemberAttribute() { Name = string.Empty; }

        public string Name { get; set; }

        // For strings only
        public CharacterSet? CharacterSet { get; set; }
        public StringPadding? StringPadding { get; set; }
        public int AllocatedStorageInBytes { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
            AllowMultiple = false, Inherited = false)]
    public sealed class H5ContractAttribute : Attribute
    {
        public H5ContractAttribute() { Name = string.Empty; }

        public string Name { get; set; }
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