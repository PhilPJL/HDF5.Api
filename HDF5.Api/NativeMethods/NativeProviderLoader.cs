﻿// <copyright file="NativeProviderLoader.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2021 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using CommunityToolkit.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

// ReSharper disable InconsistentNaming

namespace HDF5.Api.NativeMethods
{
    internal enum ProcArchitecture
    {
        X64,
        X86,
        Arm,
        Arm64
    }

    /// <summary>
    /// Helper class to load native libraries depending on the architecture of the OS and process.
    /// </summary>
    internal static class NativeProviderLoader
    {
        static readonly object StaticLock = new();

        /// <summary>
        /// Dictionary of handles to previously loaded libraries,
        /// </summary>
        static readonly Lazy<ConcurrentDictionary<string, (IntPtr handle, string path)>> NativeHandles = new(LazyThreadSafetyMode.PublicationOnly);

#if !NET5_0_OR_GREATER
        /// <summary>
        /// If the last native library failed to load then gets the corresponding exception
        /// which occurred or null if the library was successfully loaded.
        /// </summary>
        internal static Exception? LastException { get; private set; }
#endif

        static bool IsWindows { get; }
        static bool IsLinux { get; }
        static bool IsMac { get; }
        static bool IsUnix { get; }

        static ProcArchitecture ProcArchitecture { get; }
        static string Extension { get; }

        public static IEnumerable<(string name, string path)> LoadedLibraries => NativeHandles.Value.Select(kvp => ((string)kvp.Key, kvp.Value.path));

        static NativeProviderLoader()
        {
#if !NET461
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            var a = RuntimeInformation.ProcessArchitecture;
            bool arm = a == Architecture.Arm || a == Architecture.Arm64;
#else
            var p = Environment.OSVersion.Platform;
            IsLinux = p == PlatformID.Unix;
            IsMac = p == PlatformID.MacOSX;

            bool arm = false;
#endif

            IsUnix = IsLinux || IsMac;
            IsWindows = !IsUnix;

            Extension = IsWindows ? ".dll" : IsLinux ? ".so" : ".dylib";

            ProcArchitecture = Environment.Is64BitProcess
                ? arm
                    ? ProcArchitecture.Arm64
                    : ProcArchitecture.X64
                : arm
                    ? ProcArchitecture.Arm
                    : ProcArchitecture.X86;
        }

        private static string EnsureExtension(string fileName)
        {
            return string.IsNullOrEmpty(Path.GetExtension(fileName))
                ? Path.ChangeExtension(fileName, Extension)
                : fileName;
        }

        /// <summary>
        /// Load the native library with the given filename.
        /// </summary>
        /// <param name="fileName">The file name of the library to load.</param>
        /// <param name="hintPath">Hint path where to look for the native binaries. Can be empty.</param>
        /// <returns>True if the library was successfully loaded or if it has already been loaded.</returns>
        internal static bool TryLoad(string fileName, string hintPath)
        {
            Guard.IsNotNullOrEmpty(fileName);

            fileName = EnsureExtension(fileName);

            Debug.WriteLine($"TryLoad: {fileName}, hintpath='{hintPath ?? string.Empty}'");

            if (NativeHandles.Value.TryGetValue(fileName, out var library))
            {
                Debug.WriteLine($"{fileName} already loaded from {library.path}.");
                return true;
            }

            // If we have hint path provided by the user, look there first
            if (!string.IsNullOrWhiteSpace(hintPath))
            {
#pragma warning disable CS8604 // Possible null reference argument.
                if (TryLoadFromDirectory(fileName, hintPath))
                {
                    return true;
                }
#pragma warning restore CS8604 // Possible null reference argument.
            }

            // Look under the current AppDomain's base directory
            if (TryLoadFromDirectory(fileName, AppDomain.CurrentDomain.BaseDirectory))
            {
                return true;
            }

            // Look at this assembly's directory
            if (TryLoadFromDirectory(fileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to load a native library by providing its name and a directory.
        /// Tries to load an implementation suitable for the current CPU architecture
        /// and process mode if there is a matching subfolder.
        /// </summary>
        /// <returns>True if the library was successfully loaded or if it has already been loaded.</returns>
        static bool TryLoadFromDirectory(string fileName, string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            directory = Path.GetFullPath(directory);

            if (IsWindows)
            {
                return ProcArchitecture switch
                {
                    ProcArchitecture.X64 =>
                        TryLoadFile(directory, "runtimes/win-x64/native", fileName)
                        || TryLoadFile(directory, "win-x64/native", fileName)
                        || TryLoadFile(directory, "win-x64", fileName)
                        || TryLoadFile(directory, "x64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.X86 => TryLoadFile(directory, "runtimes/win-x86/native", fileName)
                        || TryLoadFile(directory, "win-x86/native", fileName)
                        || TryLoadFile(directory, "win-x86", fileName)
                        || TryLoadFile(directory, "x86", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.Arm64 =>
                        TryLoadFile(directory, "runtimes/win-arm64/native", fileName)
                        || TryLoadFile(directory, "win-arm64/native", fileName)
                        || TryLoadFile(directory, "win-arm64", fileName)
                        || TryLoadFile(directory, "arm64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.Arm =>
                        TryLoadFile(directory, "runtimes/win-arm/native", fileName)
                        || TryLoadFile(directory, "win-arm/native", fileName)
                        || TryLoadFile(directory, "win-arm", fileName)
                        || TryLoadFile(directory, "arm", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    _ => TryLoadFile(directory, string.Empty, fileName)
                };
            }

            if (IsLinux)
            {
                return ProcArchitecture switch
                {
                    ProcArchitecture.X64 =>
                        TryLoadFile(directory, "runtimes/linux-x64/native", fileName)
                        || TryLoadFile(directory, "linux-x64/native", fileName)
                        || TryLoadFile(directory, "linux-x64", fileName)
                        || TryLoadFile(directory, "x64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.X86 =>
                        TryLoadFile(directory, "runtimes/linux-x86/native", fileName)
                        || TryLoadFile(directory, "linux-x86/native", fileName)
                        || TryLoadFile(directory, "linux-x86", fileName)
                        || TryLoadFile(directory, "x86", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.Arm64 =>
                        TryLoadFile(directory, "runtimes/linux-arm64/native", fileName)
                        || TryLoadFile(directory, "linux-arm64/native", fileName)
                        || TryLoadFile(directory, "linux-arm64", fileName)
                        || TryLoadFile(directory, "arm64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.Arm =>
                        TryLoadFile(directory, "runtimes/linux-arm/native", fileName)
                        || TryLoadFile(directory, "linux-arm/native", fileName)
                        || TryLoadFile(directory, "linux-arm", fileName)
                        || TryLoadFile(directory, "arm", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    _ => TryLoadFile(directory, string.Empty, fileName)
                };
            }

            if (IsMac)
            {
                return ProcArchitecture switch
                {
                    ProcArchitecture.X64 =>
                        TryLoadFile(directory, "runtimes/osx-x64/native", fileName)
                        || TryLoadFile(directory, "osx-x64/native", fileName)
                        || TryLoadFile(directory, "osx-x64", fileName)
                        || TryLoadFile(directory, "x64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    ProcArchitecture.Arm64 =>
                        TryLoadFile(directory, "runtimes/osx-arm64/native", fileName)
                        || TryLoadFile(directory, "osx-arm64/native", fileName)
                        || TryLoadFile(directory, "osx-arm64", fileName)
                        || TryLoadFile(directory, "arm64", fileName)
                        || TryLoadFile(directory, string.Empty, fileName),
                    _ => TryLoadFile(directory, string.Empty, fileName)
                };
            }

            return ProcArchitecture switch
            {
                ProcArchitecture.X64 =>
                    TryLoadFile(directory, "x64", fileName)
                    || TryLoadFile(directory, string.Empty, fileName),
                ProcArchitecture.X86 =>
                    TryLoadFile(directory, "x86", fileName)
                    || TryLoadFile(directory, string.Empty, fileName),
                ProcArchitecture.Arm64 =>
                    TryLoadFile(directory, "arm64", fileName)
                    || TryLoadFile(directory, string.Empty, fileName),
                ProcArchitecture.Arm =>
                    TryLoadFile(directory, "arm", fileName)
                    || TryLoadFile(directory, string.Empty, fileName),
                _ => TryLoadFile(directory, string.Empty, fileName)
            };
        }

        internal static IntPtr TryGetHandle(string fileName)
        {
            Guard.IsNotNullOrEmpty(fileName);

            fileName = EnsureExtension(fileName);

            if (NativeHandles.Value.TryGetValue(fileName, out var library))
            {
                Debug.WriteLine($"TryGetHandle: {fileName} - success");
                return library.handle;
            }

            Debug.WriteLine($"TryGetHandle: {fileName} - failure");
            return IntPtr.Zero;
        }

        /// <summary>
        /// Try to load a native library by only the file name of the library.
        /// </summary>
        /// <returns>True if the library was successfully loaded or if it has already been loaded.</returns>
        private static bool TryLoadDirect(string fileName)
        {
            Guard.IsNotNullOrEmpty(fileName);
            Guard.IsNotNullOrEmpty(Path.GetExtension(fileName));

            lock (StaticLock)
            {
                if (NativeHandles.Value.TryGetValue(fileName, out var library))
                {
                    return true;
                }

#if NET5_0_OR_GREATER
                IntPtr libraryHandle = IntPtr.Zero;
                try
                {
                    if (!NativeLibrary.TryLoad(fileName, out libraryHandle) || libraryHandle == IntPtr.Zero)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                NativeHandles.Value[fileName] = (libraryHandle, fileName);
                return true;

#else
                try
                {
                    // If successful this will return a handle to the library
                    library.handle = IsWindows ? WindowsLoader.LoadLibrary(fileName) : IsMac ? MacLoader.LoadLibrary(fileName) : LinuxLoader.LoadLibrary(fileName);
                }
                catch (Exception e)
                {
                    LastException = e;
                    return false;
                }

                if (library.handle == IntPtr.Zero)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    var exception = new Win32Exception(lastError);
                    LastException = exception;
                    return false;
                }

                LastException = null;
                NativeHandles.Value[fileName] = (library.handle, fileName);
                return true;
#endif
            }
        }

        /// <summary>
        /// Try to load a native library by providing the full path including the file name of the library.
        /// </summary>
        /// <returns>True if the library was successfully loaded or if it has already been loaded.</returns>
        private static bool TryLoadFile(string directory, string relativePath, string fileName)
        {
            Guard.IsNotNullOrEmpty(fileName);
            Guard.IsNotNullOrEmpty(Path.GetExtension(fileName));
            Debug.WriteLine($"TryLoadFile {directory}, {relativePath}, {fileName}");

            lock (StaticLock)
            {
                if (NativeHandles.Value.TryGetValue(fileName, out var library))
                {
                    return true;
                }

                var fullPath = Path.GetFullPath(Path.Combine(Path.Combine(directory, relativePath), fileName));
                if (!File.Exists(fullPath))
                {
                    Debug.WriteLine($"TryLoadFile assembly doesn't exist: {fullPath}.");
                    // If the library isn't found within an architecture specific folder then return false
                    // to allow normal P/Invoke searching behavior when the library is called
                    return false;
                }

#if NET5_0_OR_GREATER
                IntPtr libraryHandle = IntPtr.Zero;
                try
                {
                    if (!NativeLibrary.TryLoad(fullPath, out libraryHandle) || libraryHandle == IntPtr.Zero)
                    {
                        Debug.WriteLine($"NativeLibrary failed to load {fileName} from {fullPath}");
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"NativeLibrary failed to load {fileName} from {fullPath}, {ex.Message}");
                    return false;
                }

                Debug.WriteLine($"NativeLibrary loaded {fileName} from {fullPath}");
                NativeHandles.Value[fileName] = (libraryHandle, fullPath);
                return true;
#else
                try
                {
                    // If successful this will return a handle to the library
                    library.handle = IsWindows ? WindowsLoader.LoadLibrary(fullPath) : IsMac ? MacLoader.LoadLibrary(fullPath) : LinuxLoader.LoadLibrary(fullPath);
                }
                catch (Exception e)
                {
                    LastException = e;
                    return false;
                }

                if (library.handle == IntPtr.Zero)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    var exception = new Win32Exception(lastError);
                    LastException = exception;
                    return false;
                }

                LastException = null;
                Debug.WriteLine($"Loader loaded {fileName} from {fullPath}");
                NativeHandles.Value[fileName] = (library.handle, fullPath);
                return true;
#endif
            }
        }

#if !NET5_0_OR_GREATER
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]
        private static class WindowsLoader
        {
            public static IntPtr LoadLibrary(string fileName)
            {
                return LoadLibraryEx(fileName, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
            }

            // Search for dependencies in the library's directory rather than the calling process's directory
            const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

            [DllImport("kernel32", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
            static extern IntPtr LoadLibraryEx(string fileName, IntPtr reservedNull, uint flags);
        }

        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]
        private static class LinuxLoader
        {
            public static IntPtr LoadLibrary(string fileName)
            {
                return dlopen(fileName, RTLD_NOW);
            }

            const int RTLD_NOW = 2;

            [DllImport("libdl.so.2", SetLastError = true)]
            static extern IntPtr dlopen(string fileName, int flags);
        }

        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]
        private static class MacLoader
        {
            public static IntPtr LoadLibrary(string fileName)
            {
                return dlopen(fileName, RTLD_NOW);
            }

            const int RTLD_NOW = 2;

            [DllImport("libdl.dylib", SetLastError = true)]
            static extern IntPtr dlopen(string fileName, int flags);
        }
#endif
    }
}
