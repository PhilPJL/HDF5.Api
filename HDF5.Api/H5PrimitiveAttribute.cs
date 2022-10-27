using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5PrimitiveAttribute<T> : H5Attribute<T> where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
/*        using var type = GetH5Type();
        using var nativeType = H5Type.GetNativeType<T>();

        var typeClass = type.GetClass();
        var nativeTypeClass = nativeType.GetClass();

*//*        if (typeClass != nativeTypeClass)
        {
            // TODO: improve exception
            throw new H5Exception($"The attribute should be of class {nativeTypeClass} but is of class {typeClass}.");
        }
*//*
        if (!type.Equals(nativeType))
        {
            // TODO: improve exception
            throw new H5Exception($"The attribute type doesn't match the expected native type of {typeof(T).Name}.");
        }
    
    
        throw new NotImplementedException();
        */
    }

#if NET7_0_OR_GREATER
    public override H5PrimitiveType<T> GetH5Type()
#else
    public override H5Type GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        return H5AAdapter.Read(this);
    }

    public override H5Attribute<T> Write([DisallowNull] T value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}
