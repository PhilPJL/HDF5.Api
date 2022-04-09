namespace HDF5Api
{
    public interface IH5TypeConverter<in TInput, out TOutput> where TOutput : struct
    {
        public H5Type CreateH5Type();
        public TOutput Convert(TInput source);
    }
}
