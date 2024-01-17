using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Core.Execution
{
    [DllMain]
    public abstract class WindowsSdkLocator : ModuleInitialize
    {
        public const string InstallationPath = "C:\\PROGRA~2\\Windows Kits\\10";

        public static readonly WindowsSdkVersion Version;

        private static readonly string BinPathx86;
        private static readonly string BinPathx64;

        private static readonly string IncludePath;

        private static readonly string UcrtLibraryPathx86;
        private static readonly string UcrtLibraryPathx64;

        private static readonly string UmLibraryPathx86;
        private static readonly string UmLibraryPathx64;

        private const string LinkerLibPath = "/LIBPATH:";

        private static readonly char[] Chars = new char[]
                                                           {
                                                               '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                                                           };
        
        static WindowsSdkLocator()
        {

            string[] windowsSdkPaths = Directory.GetDirectories(Path.Combine(InstallationPath, "Lib"));

            WindowsSdkVersion maxVersion = new WindowsSdkVersion(0, 0, 0, 0);
            WindowsSdkVersion currentVersion;

            foreach (var windowsSdkPath in windowsSdkPaths)
            {
                string dirName = windowsSdkPath.Substring(Path.GetDirectoryName(windowsSdkPath).Length + 1);

                for (int i = 0; i < dirName.Length; i++)
                {
                    if (Chars.All(aChar => aChar != dirName[i]))
                    {
                        goto SKIP;
                    }
                }

                currentVersion = new WindowsSdkVersion(dirName);

                if (currentVersion > maxVersion)
                {
                    maxVersion = currentVersion;
                }
            SKIP:
                continue;
            }

            Version = maxVersion;

            BinPathx86 = Command.GetShortPath(Path.Combine(InstallationPath, "bin", Version.Version, "x86"));
            BinPathx64 = Command.GetShortPath(Path.Combine(InstallationPath, "bin", Version.Version, "x64"));

            IncludePath = Command.GetShortPath(Path.Combine(InstallationPath, Version.Version, "include"));

            UcrtLibraryPathx86 = Command.GetShortPath(Path.Combine(InstallationPath, "Lib", Version.Version, "ucrt", "x86"));
            UcrtLibraryPathx64 = Command.GetShortPath(Path.Combine(InstallationPath, "Lib", Version.Version, "ucrt", "x64"));

            UmLibraryPathx86 = Command.GetShortPath(Path.Combine(InstallationPath, "Lib", Version.Version, "um", "x86"));
            UmLibraryPathx64 = Command.GetShortPath(Path.Combine(InstallationPath, "Lib", Version.Version, "um", "x64"));
        }

        [DllProcessAttach]
        public static void Init()
        {}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUcrtLibraryPath(bool x64 = true, string prefix = LinkerLibPath)
        {
            if (x64)
            {
                return GetUcrtLibraryPathx64(LinkerLibPath);
            }
            
            return GetUcrtLibraryPathx86(LinkerLibPath);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUcrtLibraryPathx86(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + UcrtLibraryPathx86;
            }
            
            return UcrtLibraryPathx86;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUcrtLibraryPathx64(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + UcrtLibraryPathx64;
            }
            
            return UcrtLibraryPathx64;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUmLibraryPath(bool x64 = true, string prefix = LinkerLibPath)
        {
            if (x64)
            {
                return GetUmLibraryPathx64(LinkerLibPath);
            }
            
            return GetUmLibraryPathx86(LinkerLibPath);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUmLibraryPathx86(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + UmLibraryPathx86;
            }
            
            return UmLibraryPathx86;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUmLibraryPathx64(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + UmLibraryPathx64;
            }
            
            return UmLibraryPathx64;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IEnumerable<string> GetLibraryPaths(bool x64 = true)
        {
            if (x64)
            {
                yield return UcrtLibraryPathx64;
                yield return UmLibraryPathx64;
            }
            
            yield return UcrtLibraryPathx86;
            yield return UmLibraryPathx86;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IEnumerable<string> GetLibraryPathsForLinker(bool x64 = true)
        {
            if (x64)
            {
                yield return "/LIBPATH:" + UcrtLibraryPathx64;
                yield return "/LIBPATH:" + UmLibraryPathx64;
            }
            
            yield return "/LIBPATH:" + UcrtLibraryPathx86;
            yield return "/LIBPATH:" + UmLibraryPathx86;
        }
    }
}