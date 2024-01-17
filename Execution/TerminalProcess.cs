
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;


namespace Core.Execution
{

    public class EventArgs<T> : EventArgs
    {
        public T Value { get; }

        public EventArgs(T value)
        {
            Value = value;
        }
    }

    public interface IProcessEnvironment : IDictionary<string, string>
    {

    }

    interface INodeProcess
    {
        string platform { get; }
        IProcessEnvironment env { get; }
        int getuid();
        Action nextTick();
        string versions { get; }
        string type { get; }
    }

    public interface ITerminalEnvironment : IDictionary<string, string>
    {
    }

    public interface IShellLaunchConfig
    {
        /**
         * The name of the terminal, if this is not set the name of the process will be used.
         */
        string name { get; }

        /**
         * The shell executable (bash, cmd, etc.).
         */
        string executable { get; }

        /**
         * The CLI arguments to use with executable, a string[] is in argv format and will be escaped,
         * a string is in "CommandLine" pre-escaped format and will be used as is. The string option is
         * only supported on Windows and will throw an exception if used on macOS or Linux.
         */
        string[] args { get; }

        /**
         * The current working directory of the terminal, this overrides the `terminal.integrated.cwd`
         * settings key.
         */
        string cwd { get; }

        /**
         * A custom environment for the terminal, if this is not set the environment will be inherited
         * from the VS Code process.
         */
        ITerminalEnvironment env { get; }

        /**
         * Whether to ignore a custom cwd from the `terminal.integrated.cwd` settings key (eg. if the
         * shell is being launched by an extension).
         */
        bool? ignoreConfigurationCwd { get; }

        /** Whether to wait for a key press before closing the terminal. */
        bool? stringwaitOnExit { get; }

        /**
         * A string including ANSI escape sequences that will be written to the terminal emulator
         * _before_ the terminal process has launched, a trailing \n is added at the end of the string.
         * This allows for example the terminal instance to display a styled message as the first line
         * of the terminal. Use \x1b over \033 or \e for the escape control character.
         */
        string initialText { get; }

        /**
         * When true the terminal will be created with no process. This is primarily used to give
         * extensions full control over the terminal.
         */
        bool? isRendererOnly { get; }

        /**
         * Whether the terminal process environment should be exactly as provided in
         * `TerminalOptions.env`. When this is false (default), the environment will be based on the
         * window's environment and also apply configured platform settings like
         * `terminal.integrated.windows.env` on top. When this is true, the complete environment must be
         * provided as nothing will be inherited from the process or any configuration.
         */
        bool? strictEnv { get; }
    }


    public class TerminalProcess : IDisposable
    {
        private int _exitCode;
        private object _closeTimeout;
        private IPty _ptyProcess;
        private string _currentTitle = "";
        private Action _processStartupComplete;
        private bool _isDisposed = false;
        private Timer _titleInterval = null;
        private string _initialCwd;


        public EventHandler<EventArgs<string>> onProcessData;

        public EventHandler<EventArgs<int>> onProcessExit;


        public EventHandler<EventArgs<int>> onProcessIdReady;


        public EventHandler<EventArgs<string>> onProcessTitleChanged;


        public TerminalProcess(IShellLaunchConfig shellLaunchConfig, string cwd, int cols, int rows, IProcessEnvironment env, bool windowsEnableConpty)
        {
            string shellName = shellLaunchConfig.executable;


            _initialCwd = cwd;
            //const useConpty = windowsEnableConpty && process.platform === "win32" && _getWindowsBuildNumber() >= 18309;

            IPtyOptions options = new PtyOptions(shellName, cwd, env, cols, rows,);

            try
            {
                _ptyProcess = pty.spawn(shellLaunchConfig.executable, shellLaunchConfig.args, options);

                _processStartupComplete = new Promise<void>(c =>
                {
                    onProcessIdReady((pid) =>
                    {
                        c();
                    });
                });
            }
            catch (Exception error)
            {
                // The only time this is expected to happen is when the file specified to launch with does not exist.
                _exitCode = 2;
                _queueProcessExit();
                _processStartupComplete = Promise.resolve(undefined);
                return;
            }

            //_ptyProcess.on("data", (data) =>
            //{
            //    _onProcessData.fire(data);
            //    if (_closeTimeout)
            //    {
            //        clearTimeout(_closeTimeout);
            //        _queueProcessExit();
            //    }
            //});

            //_ptyProcess.on("exit", (code) =>
            //{
            //    _exitCode = code;
            //    _queueProcessExit();
            //});

            //// TODO  We should no longer need to delay this since pty.spawn is sync
            //setTimeout(() =>
            //{
            //    _sendProcessId();
            //}, 500);
            //_setupTitlePolling();
        }

        public void Dispose()
        {
            //_isDisposed = true;
            //if (_titleInterval)
            //{
            //    clearInterval(_titleInterval);
            //}
            //_titleInterval = null;
            //_onProcessData.Dispose();
            //_onProcessExit.Dispose();
            //_onProcessIdReady.Dispose();
            //_onProcessTitleChanged.Dispose();
        }

        private int _getWindowsBuildNumber()
        {
            //const osVersion = (/ (\d +)\.(\d +)\.(\d +)/ g).exec(os.release());
            //let buildNumber  number = 0;
            //if (osVersion && osVersion.length === 4)
            //{
            //    buildNumber = parseInt(osVersion[3]);
            //}
            return 120;
        }

        private void _setupTitlePolling()
        {
            //// Send initial timeout async to give event listeners a chance to init
            //setTimeout(() =>
            //{
            //    _sendProcessTitle();
            //}, 0);
            //// Setup polling
            //_titleInterval = setInterval(() =>
            //{
            //    if (_currentTitle != _ptyProcess.process)
            //    {
            //        _sendProcessTitle();
            //    }
            //}, 200);
        }

        // Allow any trailing data events to be sent before the exit event is sent.
        // See https //github.com/Tyriar/node-pty/issues/72
        private void _queueProcessExit()
        {
            //if (_closeTimeout)
            //{
            //    clearTimeout(_closeTimeout);
            //}
            //_closeTimeout = setTimeout(() => _kill(), 250);
        }

        private void _kill()
        {
            //// Wait to kill to process until the start up code has run. This prevents us from firing a process exit before a
            //// process start.
            //_processStartupComplete.then(() =>
            //{
            //    if (_isDisposed)
            //    {
            //        return;
            //    }
            //// Attempt to kill the pty, it may have already been killed at this
            //// point but we want to make sure
            //try
            //    {
            //        _ptyProcess.kill();
            //    }
            //    catch (ex)
            //    {
            //    // Swallow, the pty has already been killed
            //}
            //    _onProcessExit.fire(_exitCode);
            //    dispose();
            //});
        }

        private void _sendProcessId()
        {
            // _onProcessIdReady.fire(_ptyProcess.pid);
        }

        private void _sendProcessTitle()
        {
            //if (_isDisposed)
            //{
            //    return;
            //}
            //_currentTitle = _ptyProcess.process;
            //_onProcessTitleChanged.fire(_currentTitle);
        }

        public void shutdown(bool immediate)
        {
            //if (immediate)
            //{
            //    _kill();
            //}
            //else
            //{
            //    _queueProcessExit();
            //}
        }

        public void input(string data)
        {
            if (_isDisposed)
            {
                return;
            }
            _ptyProcess.write(data);
        }

        public void resize(int cols, int rows)
        {
            if (_isDisposed)
            {
                return;
            }
            _ptyProcess.resize(Math.Max(cols, 1), Math.Max(rows, 1));
        }

        public string getInitialCwd()
        {
            return _initialCwd;
        }

        public string getCwd()
        {
            //if (platform.isWindows)
            //{
            //    return new Promise<string>(resolve =>
            //    {
            //        resolve(_initialCwd);
            //    });
            //}

            //return new Promise<string>(resolve =>
            //{
            //    exec("lsof -p " + _ptyProcess.pid + " | grep cwd", (error, stdout, stderr) =>
            //    {
            //        if (stdout !== "")
            //        {
            //            resolve(stdout.substring(stdout.indexOf("/"), stdout.length - 1));
            //        }
            //    });
            //});
        }

    }

}
