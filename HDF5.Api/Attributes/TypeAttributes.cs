namespace HDF5.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class H5IncludeAttribute : Attribute
    {
        public H5IncludeAttribute()
        {
            Title = string.Empty;
        }

        public string Title { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class H5IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class H5StringAttribute : H5IncludeAttribute
    {
        public H5StringAttribute()
        {
            CharacterSet = CharacterSet.Utf8;
            StringPadding = StringPadding.NullTerminate;
        }

        public CharacterSet CharacterSet { get; set; }
        public StringPadding StringPadding { get; set; }
        public int AllocatedStorageInBytes { get; }
    }
}
