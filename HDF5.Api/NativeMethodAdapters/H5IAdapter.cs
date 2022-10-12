using System.IO;
using static HDF5.Api.NativeMethods.H5I;
namespace HDF5.Api.NativeMethodAdapters;

internal static class H5IAdapter
{
    internal static string GetName<T>(H5Object<T> location) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.DataSet, HandleType.Group);//, HandleType.NamedDataType);

        // TODO: duplicates H5File.GetName
#if NET7_0_OR_GREATER
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
#else
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
#endif
    }
}
