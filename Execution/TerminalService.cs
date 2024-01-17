
using System.Linq;

namespace Core.Execution
{

    public static class TerminalService
    {

        //private const string TERMINAL_TITLE = "MINGW64";

        //public _serviceBrand: any;

        private const string CMD = "cmd.exe";

        //constructor(
        //	@IConfigurationService private readonly _configurationService: IConfigurationService
        //) {
        //}

        public static void OpenTerminal(string command)
        {
            //const configuration = this._configurationService.getValue<ITerminalConfiguration>();

            //SpawnTerminal(cp, configuration, processes.getWindowsShell(), command);
        }

        public static void RunInTerminal(string title, string dir, string[] args)//, IProcessEnvironment envVars )
        {

            //const configuration = this._configurationService.getValue<ITerminalConfiguration>();
            //const terminalConfig = configuration.terminal.external;
            const string exec = @"C:\POSIX\usr\bin\mintty.exe";



            string TERMINAL_TITLE = $"{dir} - {title}";
            string command = $"{string.Join(" ", args)} & pause";

            string[] cmdArgs = new[] { "/c", "start", $"\"{title}\"", "/wait", exec, "/c", command };

            //        // merge environment variables into a copy of the process.env
            //        string env = assign({ }, process.env, envVars);

            //        // delete environment variables that have a null value
            //        Object.keys(env).filter(v => env[v] === null).forEach(key => delete env[key]);

            //        const options: any = {
            //        cwd: dir,
            //env: env,
            //windowsVerbatimArguments: true

            //        };

            //        const cmd = cp.spawn(CMD, cmdArgs, options);
            //        cmd.on("error", e);

            //        c(undefined);

        }

        //private static void SpawnTerminal(spawner, ITerminalConfiguration configuration, string command, string workingDirectory)
        //{
        //const terminalConfig  = configuration.terminal.external;
        //const exec            = terminalConfig.windowsExec || getDefaultTerminalWindows();
        //const spawnType       = this.getSpawnType(exec);

        //// Make the drive letter uppercase on Windows (see #9448)
        //if (cwd && cwd[1] == = ":")
        //{
        //    cwd = cwd[0].toUpperCase() + cwd.substr(1);
        //}

        //// cmder ignores the environment cwd and instead opts to always open in %USERPROFILE%
        //// unless otherwise specified
        //if (spawnType == = WinSpawnType.CMDER)
        //{
        //    spawner.spawn(exec, [cwd]);
        //    return Promise.resolve(undefined);
        //}

        //const cmdArgs  = ["/c", "start", "/wait"];

        //if (exec.indexOf(" ") >= 0)
        //{
        //    // The "" argument is the window title. Without this, exec doesn"t work when the path
        //    // contains spaces
        //    cmdArgs.push("""");
        //}

        //cmdArgs.push(exec);

        //return new Promise<void>((c, e) =>
        //                         {
        //                             const env  = cwd ? {
        //                             cwd:
        //                                 cwd

        //                             } : undefined;
        //                             const child  = spawner.spawn(command, cmdArgs, env);
        //                             child.on("error", e);
        //                             child.on("exit", () => c());
        //                         });
        //}
    }
}
