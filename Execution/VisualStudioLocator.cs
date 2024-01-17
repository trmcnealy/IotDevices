using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Core.Execution
{
    [DllMain]
    public abstract class VisualStudioLocator : ModuleInitialize
    {
        private const string VsWhere = "C:\\PROGRA~2\\Microsoft Visual Studio\\Installer\\vswhere.exe";

        private static readonly IEnumerable<string> VsWhereParams = new []{"-latest", "-nologo"};
        
        private const string InstallationPathName = "installationPath";

        public static readonly string InstallationPath;

        private const string MsvcRelativePath = "VC\\Tools\\MSVC";

        public static readonly string MsvcPath;

        public static readonly VisualStudioVersion MsvcVersion;

        private const string Binx86RelativePath = "bin\\Hostx86\\x86";
        private const string Binx64RelativePath = "bin\\Hostx64\\x64";

        private static readonly string BinPathx86;
        private static readonly string BinPathx64;

        private static readonly string IncludePath;

        private static readonly string LibraryPathx86;
        private static readonly string LibraryPathx64;

        private const string BscmakeFileName  = "bscmake.exe";
        private const string ClFileName       = "cl.exe";
        private const string CvtresFileName   = "cvtres.exe";
        private const string DumpbinFileName  = "dumpbin.exe";
        private const string EditbinFileName  = "editbin.exe";
        private const string LibFileName      = "lib.exe";
        private const string LinkFileName     = "link.exe";
        private const string Ml64FileName     = "ml64.exe";
        private const string MspdbcmfFileName = "mspdbcmf.exe";
        private const string MspdbsrvFileName = "mspdbsrv.exe";
        private const string NmakeFileName    = "nmake.exe";
        private const string PgocvtFileName   = "pgocvt.exe";
        private const string PgomgrFileName   = "pgomgr.exe";
        private const string PgosweepFileName = "pgosweep.exe";
        private const string UndnameFileName  = "undname.exe";
        private const string VctipFileName    = "vctip.exe";
        private const string XdcmakeFileName  = "xdcmake.exe";

        private const string LinkerLibPath = "/LIBPATH:";

        static VisualStudioLocator()
        {
            string[] output = Command.Execute(VsWhere, VsWhereParams).Result;

            foreach (var line in output)
            {
                if (line.StartsWith(InstallationPathName))
                {
                    InstallationPath = Command.GetShortPath(line.Substring(InstallationPathName.Length + 2));
                    break;
                }
            }

            MsvcPath = Command.GetShortPath(Path.Combine(InstallationPath, MsvcRelativePath));


            string[] msvcPaths = Directory.GetDirectories(MsvcPath);

            VisualStudioVersion maxVersion = new VisualStudioVersion(0, 0, 0);
            VisualStudioVersion currentVersion;

            foreach (var msvcPath in msvcPaths)
            {
                currentVersion = new VisualStudioVersion(msvcPath.Substring(Path.GetDirectoryName(msvcPath).Length + 1));

                if (currentVersion > maxVersion)
                {
                    maxVersion = currentVersion;
                }
            }

            MsvcVersion = maxVersion;

            BinPathx86 = Command.GetShortPath(Path.Combine(MsvcPath, MsvcVersion.Version, Binx86RelativePath));
            BinPathx64 = Command.GetShortPath(Path.Combine(MsvcPath, MsvcVersion.Version, Binx64RelativePath));

            IncludePath = Command.GetShortPath(Path.Combine(MsvcPath, MsvcVersion.Version, "include"));

            LibraryPathx86 = Command.GetShortPath(Path.Combine(MsvcPath, MsvcVersion.Version, "lib\\x86"));
            LibraryPathx64 = Command.GetShortPath(Path.Combine(MsvcPath, MsvcVersion.Version, "lib\\x64"));
        }

        [DllProcessAttach]
        public static void Init()
        {}
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetBscmake(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, BscmakeFileName);
            }

            return Path.Combine(BinPathx86, BscmakeFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCl(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, ClFileName);
            }

            return Path.Combine(BinPathx86, BscmakeFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCvtres(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, CvtresFileName);
            }

            return Path.Combine(BinPathx86, CvtresFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetDumpbin(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, DumpbinFileName);
            }

            return Path.Combine(BinPathx86, DumpbinFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetEditbin(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, EditbinFileName);
            }

            return Path.Combine(BinPathx86, EditbinFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetLib(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, LibFileName);
            }

            return Path.Combine(BinPathx86, LibFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetLink(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, LinkFileName);
            }

            return Path.Combine(BinPathx86, LinkFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetMl64(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, Ml64FileName);
            }

            return Path.Combine(BinPathx86, Ml64FileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetMspdbcmf(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, MspdbcmfFileName);
            }

            return Path.Combine(BinPathx86, MspdbcmfFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetMspdbsrv(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, MspdbsrvFileName);
            }

            return Path.Combine(BinPathx86, MspdbsrvFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetNmake(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, NmakeFileName);
            }

            return Path.Combine(BinPathx86, NmakeFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetPgocvt(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, PgocvtFileName);
            }

            return Path.Combine(BinPathx86, PgocvtFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetPgomgr(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, PgomgrFileName);
            }

            return Path.Combine(BinPathx86, PgomgrFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetPgosweep(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, PgosweepFileName);
            }

            return Path.Combine(BinPathx86, PgosweepFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUndname(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, UndnameFileName);
            }

            return Path.Combine(BinPathx86, UndnameFileName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetVctip(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, VctipFileName);
            }

            return Path.Combine(BinPathx86, VctipFileName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetXdcmake(bool x64 = true)
        {
            if (x64)
            {
                return Path.Combine(BinPathx64, XdcmakeFileName);
            }

            return Path.Combine(BinPathx86, XdcmakeFileName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetLibraryPath(bool x64 = true, string prefix = LinkerLibPath)
        {
            if (x64)
            {
                return GetLibraryPathx64(LinkerLibPath);
            }

            return GetLibraryPathx86(LinkerLibPath);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetLibraryPathx86(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + LibraryPathx86;
            }

            return LibraryPathx86;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetLibraryPathx64(string prefix = LinkerLibPath)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return LinkerLibPath + LibraryPathx64;
            }

            return LibraryPathx64;
        }
    }
}