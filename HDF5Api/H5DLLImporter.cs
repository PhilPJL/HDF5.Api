//using System;

//namespace HDF5Api;

//internal delegate T Converter<T>(IntPtr address);

///// <summary>
///// Helper class used to fetch public variables (e.g. native type values)
///// exported by the HDF5 DLL
///// </summary>
//internal abstract class H5DLLImporter
//{
//    public static readonly H5DLLImporter Instance;

//    static H5DLLImporter()
//    {
//        _ = H5.open();

//        switch (Environment.OSVersion.Platform)
//        {
//            case PlatformID.Win32NT:
//            case PlatformID.Win32S:
//            case PlatformID.Win32Windows:
//            case PlatformID.WinCE:
//                Instance = new H5WindowsDLLImporter(Constants.DLLFileName);
//                break;
//            case PlatformID.Xbox:
//            case PlatformID.MacOSX:
//            case PlatformID.Unix:
//                Instance = new H5UnixDllImporter(Constants.DLLFileName);
//                break;
//            default:
//                throw new NotImplementedException(); ;
//        }
//    }

//    protected abstract IntPtr _GetAddress(string varName);

//    public IntPtr GetAddress(string varName)
//    {
//        var address = _GetAddress(varName);
//        if (address == IntPtr.Zero)
//            throw new Exception(string.Format("The export with name \"{0}\" doesn't exist.", varName));
//        return address;
//    }

//    public bool GetAddress(string varName, out IntPtr address)
//    {
//        address = _GetAddress(varName);
//        return (address == IntPtr.Zero);
//    }

//    /*public bool GetValue<T>(
//        string          varName,
//        ref T           value,
//        Func<IntPtr, T> converter
//        )
//    {
//        IntPtr address;
//        if (!this.GetAddress(varName, out address))
//            return false;
//        value = converter(address);
//        return true;

//        //return (T) Marshal.PtrToStructure(address,typeof(T));
//    }*/

//    public unsafe hid_t GetHid(string varName)
//    {
//        return *(hid_t*)this.GetAddress(varName);
//    }
//}

//#region Windows Importer
//internal partial class H5WindowsDLLImporter : H5DLLImporter
//{
//    [LibraryImport("kernel32.dll", SetLastError = true)]
//    internal static partial IntPtr GetModuleHandle(string lpszLib);

//    [LibraryImport("kernel32.dll", SetLastError = true)]
//    internal static partial IntPtr GetProcAddress        (IntPtr hModule, string procName);

//    [LibraryImport("kernel32.dll", SetLastError = true)]
//    internal static partial IntPtr LoadLibrary(string lpszLib);

//    private readonly IntPtr hLib;

//    public H5WindowsDLLImporter(string libName)
//    {
//        hLib = GetModuleHandle(libName);
//        if (hLib == IntPtr.Zero)  // the library hasn't been loaded
//        {
//            hLib = LoadLibrary(libName);
//            if (hLib == IntPtr.Zero)
//            {
//                try
//                {
//                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
//                }
//                catch (Exception e)
//                {
//                    throw new Exception(string.Format("Couldn't load library \"{0}\"", libName), e);
//                }
//            }
//        }
//    }

//    protected override IntPtr _GetAddress(string varName)
//    {
//        return GetProcAddress(hLib, varName);
//    }
//}
//#endregion

//internal partial class H5UnixDllImporter : H5DLLImporter
//{

//    [LibraryImport("libdl.so")]
//    protected static partial IntPtr dlopen(string filename, int flags);

//    [LibraryImport("libdl.so")]
//    protected static partial IntPtr dlsym(IntPtr handle, string symbol);

//    [LibraryImport("libdl.so")]
//    protected static partial IntPtr dlerror();

//    private readonly IntPtr hLib;

//    public H5UnixDllImporter(string libName)
//    {
//        if (libName == "hdf5.dll")
//        {
//            libName = "/usr/lib/libhdf5.so";

//        }
//        if (libName == "hdf5_hd.dll")
//        {
//            libName = "/usr/lib/libhdf5_hl.so";
//        }



//        hLib = dlopen(libName, RTLD_NOW);
//        if (hLib == IntPtr.Zero)
//        {
//            throw new ArgumentException(
//                String.Format(
//                    "Unable to load unmanaged module \"{0}\"",
//                    libName));
//        }
//    }

//    const int RTLD_NOW = 2; // for dlopen's flags
//    protected override IntPtr _GetAddress(string varName)
//    {
//        var address = dlsym(hLib, varName);
//        var errPtr = dlerror();
//        if (errPtr != IntPtr.Zero)
//        {
//            throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
//        }

//        return address;
//    }
//}
