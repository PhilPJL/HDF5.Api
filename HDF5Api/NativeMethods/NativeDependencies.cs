/*using System.Configuration;

namespace HDF.PInvoke;

internal static class NativeDependencies
{
    public const string NativePathSetting = "NativeDependenciesAbsolutePath";

    public static void ResolvePathToExternalDependencies()
    {
        if (!GetDllPathFromAppConfig(out var NativeDllPath))
        {
            GetDllPathFromAssembly(out NativeDllPath);
        }

        AddPathStringToEnvironment(NativeDllPath);
    }

    //----------------------------------------------------------------

    private static bool GetDllPathFromAppConfig(out string aPath)
    {
        aPath = string.Empty;
        try
        {
            if (ConfigurationManager.AppSettings.Count <= 0) return false;
            string pathFromAppSettings = ConfigurationManager.
                AppSettings[NativePathSetting].ToString();
            if (string.IsNullOrEmpty(pathFromAppSettings))
                return false;

            foreach (var c in Path.GetInvalidPathChars())
            {
                if (pathFromAppSettings.Contains(new string(c, 1)))
                    return false;
            }

            aPath = pathFromAppSettings;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetAssemblyName()
    {
        string myPath = new Uri(System.Reflection.Assembly
            .GetExecutingAssembly().CodeBase).AbsolutePath;
        myPath = Uri.UnescapeDataString(myPath);
        return myPath;
    }

    private static void GetDllPathFromAssembly(out string aPath)
    {
        aPath = IntPtr.Size switch
        {
            8 => Path.Combine(Path.GetDirectoryName(GetAssemblyName()), Constants.DLL64bitPath),
            4 => Path.Combine(Path.GetDirectoryName(GetAssemblyName()), Constants.DLL32bitPath),
            _ => throw new NotImplementedException(),
        };
    }

    private static void AddPathStringToEnvironment(string aPath)
    {
        try
        {
            string EnvPath = Environment.GetEnvironmentVariable("PATH");
            if (EnvPath.Contains(aPath)) return;

            Environment.SetEnvironmentVariable("PATH", aPath + ";" + EnvPath);


            System.Diagnostics.Trace.WriteLine(string.Format(
                "{0} added to Path.", aPath));
        }
        catch (SecurityException)
        {
            System.Diagnostics.Trace.WriteLine(
                "Changing PATH not allowed");
        }
    }
}
*/