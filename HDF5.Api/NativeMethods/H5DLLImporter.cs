/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Diagnostics;

namespace HDF5.Api.NativeMethods;

// NOTE: native module loading now done using NativeProviderLoader from MathNet

internal abstract class H5DLLImporter
{
    private static H5DLLImporter? _instance;
    public static H5DLLImporter Instance => _instance ??= Initialise();

    private static H5DLLImporter Initialise()
    {
        if (_instance != null)
            return _instance;

        if (H5.open() < 0)
            throw new Exception("Could not initialize HDF5 library.");

#if NET5_0_OR_GREATER
        return new H5NativeImporter();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new H5LinuxDllImporter();

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return new H5MacDllImporter();

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new H5WindowsDLLImporter();

        else
            throw new PlatformNotSupportedException();
#endif
    }

    protected IntPtr ModuleHandle { get; private set; }

    protected H5DLLImporter()
    {
        ModuleHandle = NativeProviderLoader.TryGetHandle(Constants.DLLFileName);

        if (ModuleHandle == IntPtr.Zero)
            throw new DllNotFoundException(Constants.DLLFileName);
    }

    protected abstract IntPtr InternalGetAddress(string varName);

    public IntPtr GetAddress(string varName)
    {
        var address = InternalGetAddress(varName);
        if (address == IntPtr.Zero)
            throw new Exception(string.Format("The export with name \"{0}\" doesn't exist.", varName));
        return address;
    }

    public unsafe hid_t GetHid(string varName)
    {
        return *(hid_t*)GetAddress(varName);
    }
}

#if NET5_0_OR_GREATER

internal class H5NativeImporter : H5DLLImporter
{
    public H5NativeImporter ()
	{
        Debug.WriteLine("Init: H5NativeImporter");
	}

    protected override IntPtr InternalGetAddress(string varName)
    {
        if (NativeLibrary.TryGetExport(ModuleHandle, varName, out nint address))
        {
            return address;
        }

        throw new Exception(string.Format("The export with name \"{0}\" doesn't exist.", varName));
    }
}

#else

internal class H5WindowsDLLImporter : H5DLLImporter
{
    public H5WindowsDLLImporter()
    {
        Debug.WriteLine("Init: H5WindowsDLLImporter");
    }

    [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, SetLastError = true)]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

    protected override IntPtr InternalGetAddress(string varName)
    {
        return GetProcAddress(ModuleHandle, varName);
    }
}

internal class H5LinuxDllImporter : H5DLLImporter
{
    [DllImport("libdl.so.2")]
    protected static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so.2")]
    protected static extern IntPtr dlerror();

    protected override IntPtr InternalGetAddress(string varName)
    {
        var address = dlsym(ModuleHandle, varName);
        var errPtr = dlerror();

        if (errPtr != IntPtr.Zero)
            throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));

        return address;
    }
}

internal class H5MacDllImporter : H5DLLImporter
{
    [DllImport("libdl.dylib")]
    protected static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.dylib")]
    protected static extern IntPtr dlerror();

    protected override IntPtr InternalGetAddress(string varName)
    {
        var address = dlsym(ModuleHandle, varName);
        var errPtr = dlerror();

        if (errPtr != IntPtr.Zero)
            throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));

        return address;
    }
}

#endif