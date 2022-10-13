using System.IO;

namespace HDF5.Api.NativeMethodAdapters
{
    internal static class Helpers
    {
/*#if NET7_0_OR_GREATER
        internal static string GetName<T>(H5Object<T> location, Func<long, Span<byte>, int,int> get_name)
             where T : H5Object<T>
        {
            {
            int length = (int)get_name(location, new Span<byte>(), 0);
            length.ThrowIfError();
            if (length > 261)
            {
                throw new PathTooLongException();
            }
            if (length <= 1)
            {
                throw new Hdf5Exception("Name length returned was too short");
            }
            Span<byte> buffer = stackalloc byte[length + 1];
            var err = (int)get_name(location, buffer, length + 1);
            err.ThrowIfError();
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
#endif

#if NETSTANDARD
        internal static string GetName<T>()
        {
            int length = (int)get_name(location, null!, new IntPtr(0));
            length.ThrowIfError();
            if (length > 261)
            {
                throw new PathTooLongException();
            }
            if (length <= 1)
            {
                throw new Hdf5Exception("Name length returned was too short");
            }
            var name = new StringBuilder(length + 1);
            var err = get_name(location, name, new IntPtr(length + 1));
            ((int)err).ThrowIfError();
            return name.ToString();
        }
#endif*/
    }
}
