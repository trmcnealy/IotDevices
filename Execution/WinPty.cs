using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using Microsoft.Win32.SafeHandles;

using Core.Win32;

namespace Core.Execution
{

    public class WinPtyDataEventArgs : EventArgs
    {
        public string Data { get; }

        public WinPtyDataEventArgs(string data)
        {
            Data = data;
        }
    }

    public class WinPtyExitEventArgs
    {
        public int ExitCode { get; }
        public int? Signal { get; }

        public WinPtyExitEventArgs(int exitCode, int? signal)
        {
            ExitCode = exitCode;
            Signal = signal;
        }
    }

    public delegate void WinPtyDataEventHandler(object sender, WinPtyDataEventArgs e);

    public delegate void WinPtyExitEventHandler(object sender, WinPtyExitEventArgs e);


    public interface IPtyOptions
    {
        string Name { get; set; }

        int? Cols { get; set; }

        int? Rows { get; set; }

        string WorkingDirectory { get; set; }

        Dictionary<string, string> Environment { get; set; }

        int? Uid { get; set; }

        int? Gid { get; set; }

        string Encoding { get; set; }

        bool? ExperimentalUseConpty { get; set; }
    }

    public sealed class PtyOptions : IPtyOptions
    {
        public string Name { get; set; }
        public int? Cols { get; set; }
        public int? Rows { get; set; }
        public string WorkingDirectory { get; set; }
        public Dictionary<string, string> Environment { get; set; }
        public int? Uid { get; set; }
        public int? Gid { get; set; }
        public string Encoding { get; set; }
        public bool? ExperimentalUseConpty { get; set; }

        public PtyOptions(string name, int? cols, int? rows, string workingDirectory, Dictionary<string, string> environment)
        {
            Name = name;
            Cols = cols;
            Rows = rows;
            WorkingDirectory = workingDirectory;
            Environment = environment;
        }
    }

    public interface IPty
    {
        /**
         * The process ID of the outer process.
         */
        int pid { get; }

        /**
         * The title of the active process.
         */
        string process { get; }

        /**
         * Adds a listener to the data event, fired when data is returned from the pty.
         * @param event The name of the event.
         * @param listener The callback function.
         */
        void on(WinPtyDataEventArgs e);

        /**
         * Adds a listener to the exit event, fired when the pty exits.
         * @param event The name of the event.
         * @param listener The callback function, exitCode is the exit code of the process and signal is
         * the signal that triggered the exit. signal is not supported on Windows.
         */
        void on(WinPtyExitEventHandler e);

        /**
         * Resizes the dimensions of the pty.
         * @param columns The number of columns to use.
         * @param rows The number of rows to use.
         */
        void resize(int columns, int rows);

        /**
         * Writes data to the pty.
         * @param data The data to write.
         */
        void write(string data);

        /**
         * Kills the pty.
         * @param signal The signal to use, defaults to SIGHUP. If the TIOCSIG/TIOCSIGNAL ioctl is not
         * supported then the process will be killed instead. This parameter is not supported on
         * Windows.
         * @throws Will throw when signal is used on Windows.
         */
        void kill(string signal);
    }

    public sealed class WinPtyException : Exception
    {
        public uint ErrorCode { get; }

        public WinPtyException(IntPtr error)
            : base(WinPty.Native.ErrorMessage(error))
        {
            ErrorCode = WinPty.Native.ErrorCode(error);
        }

        public static void Throw()
        {
            throw new WinPtyException(IntPtr.Zero);
        }

        public static void Throw(IntPtr error)
        {
            throw new WinPtyException(error);
        }
    }

    internal sealed class SafeThreadHandle : SafeHandle
    {
        internal SafeThreadHandle()
            : base(new IntPtr(0), true)
        {
        }

        internal void InitialSetHandle(IntPtr h)
        {
            Debug.Assert(IsInvalid, "Safe handle should only be set once");
            base.SetHandle(h);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero || handle == new IntPtr(-1); }
        }

        protected override bool ReleaseHandle()
        {
            return Kernel32.Native.CloseHandle(handle);
        }
    }

    public sealed class WinPty : IDisposable
    {
        internal class ConstLPWStrMarshaler : ICustomMarshaler
        {
            private static readonly ICustomMarshaler Instance = new ConstLPWStrMarshaler();

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return Instance;
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return Marshal.PtrToStringUni(pNativeData);
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
            }

            public int GetNativeDataSize()
            {
                throw new NotSupportedException();
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                throw new NotSupportedException();
            }

            public void CleanUpManagedData(object ManagedObj)
            {
                throw new NotSupportedException();
            }
        }

        internal static class Native
        {
            private const string DllName = "winpty.dll";

            public const int ERROR_SUCCESS = 0;
            public const int ERROR_OUT_OF_MEMORY = 1;
            public const int ERROR_SPAWN_CREATE_PROCESS_FAILED = 2;
            public const int ERROR_LOST_CONNECTION = 3;
            public const int ERROR_AGENT_EXE_MISSING = 4;
            public const int ERROR_UNSPECIFIED = 5;
            public const int ERROR_AGENT_DIED = 6;
            public const int ERROR_AGENT_TIMEOUT = 7;
            public const int ERROR_AGENT_CREATION_FAILED = 8;
            public const ulong FLAG_CONERR = 0x1ul;
            public const ulong FLAG_PLAIN_OUTPUT = 0x2ul;
            public const ulong FLAG_COLOR_ESCAPES = 0x4ul;
            public const ulong FLAG_ALLOW_CURPROC_DESKTOP_CREATION = 0x8ul;

            public const ulong FLAG_MASK = 0ul | FLAG_CONERR | FLAG_PLAIN_OUTPUT | FLAG_COLOR_ESCAPES | FLAG_ALLOW_CURPROC_DESKTOP_CREATION;

            public const ulong MOUSE_MODE_NONE = 0;
            public const ulong MOUSE_MODE_AUTO = 1;
            public const ulong MOUSE_MODE_FORCE = 2;
            public const ulong SPAWN_FLAG_AUTO_SHUTDOWN = 1ul;
            public const ulong SPAWN_FLAG_EXIT_AFTER_SHUTDOWN = 2ul;
            public const ulong SPAWN_FLAG_MASK = 0ul | SPAWN_FLAG_AUTO_SHUTDOWN | SPAWN_FLAG_EXIT_AFTER_SHUTDOWN;


            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_error_code", PreserveSig = true, ExactSpelling = true, SetLastError = false)]
            public static extern uint ErrorCode(IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_error_msg", ExactSpelling = true, SetLastError = false)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public static extern string ErrorMessage(IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_error_free", ExactSpelling = true, SetLastError = false)]
            public static extern void ErrorFree(IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_config_new", ExactSpelling = true, SetLastError = false)]
            public static extern IntPtr ConfigNew(ulong agentFlags, out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_config_free", ExactSpelling = true, SetLastError = false)]
            public static extern void ConfigFree(IntPtr cfg);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_config_set_initial_size", ExactSpelling = true, SetLastError = false)]
            public static extern void ConfigSetInitialSize(IntPtr cfg, int cols, int rows);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_config_set_mouse_mode", ExactSpelling = true, SetLastError = false)]
            public static extern void ConfigSetMouseMode(IntPtr cfg, int mouseMode);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_config_set_agent_timeout", ExactSpelling = true, SetLastError = false)]
            public static extern void ConfigSetAgentTimeout(IntPtr cfg, uint timeoutMs);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_open", ExactSpelling = true, SetLastError = false)]
            public static extern IntPtr Open(IntPtr cfg, out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_agent_process", ExactSpelling = true, SetLastError = false)]
            public static extern IntPtr AgentProcess(IntPtr wp);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_conin_name", ExactSpelling = true, SetLastError = false)]
            [return: MarshalAs(UnmanagedType.LPWStr)] //UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstLPWStrMarshaler)
            public static extern string ConinName(IntPtr wp);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_conout_name", ExactSpelling = true, SetLastError = false)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public static extern string ConoutName(IntPtr wp);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_conerr_name", ExactSpelling = true, SetLastError = false)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public static extern string ConerrName(IntPtr wp);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_spawn_config_new", ExactSpelling = true, SetLastError = false)]
            public static extern IntPtr SpawnConfigNew(ulong spawnFlags,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string appname,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string cmdline,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string cwd,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string env,
                                                       out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_spawn_config_free", ExactSpelling = true, SetLastError = false)]
            public static extern void SpawnConfigFree(IntPtr cfg);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_spawn", ExactSpelling = true, SetLastError = false)]
            public static extern bool Spawn(IntPtr wp,
                                            IntPtr cfg,
                                            out SafeProcessHandle process_handle,
                                            out SafeThreadHandle thread_handle,
                                            out uint create_process_error,
                                            out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_set_size", ExactSpelling = true, SetLastError = false)]
            public static extern bool SetSize(IntPtr wp, int cols, int rows, out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_get_console_process_list", ExactSpelling = true, SetLastError = false)]
            public static extern int GetConsoleProcessList(IntPtr wp,
                                                           int[] processList,
                                                           int processCount,
                                                           out IntPtr err);

            [SuppressUnmanagedCodeSecurity, MethodImpl(MethodImplOptions.AggressiveInlining), DllImport(DllName, CharSet = CharSet.Unicode, PreserveSig = true, EntryPoint = "winpty_free", ExactSpelling = true, SetLastError = false)]
            public static extern void Free(IntPtr wp);
        }

        private IntPtr handle;
        private IntPtr err;
        private IntPtr cfg;
        private IntPtr spawnCfg;

        private StreamWriter stdin = null;
        private StreamReader stdout = null;
        private StreamReader stderr = null;

        private AsyncStreamReader _output;

        public event WinPtyDataEventHandler DataEvent;

        public event WinPtyExitEventHandler ExitEvent;

        public uint Pid { get; set; }
        public uint Tid { get; set; }

        public string Title { get; set; }

        public volatile bool Exited = false;

        public WinPty(string exe, string[] args, string workingDirectory, Dictionary<string, string> environment)
        {

            handle = IntPtr.Zero;
            err = IntPtr.Zero;
            cfg = IntPtr.Zero;
            spawnCfg = IntPtr.Zero;

            //try
            //{

                cfg = Native.ConfigNew(Native.FLAG_COLOR_ESCAPES, out err);
                Native.ConfigSetInitialSize(cfg, 80, 32);

                handle = Native.Open(cfg, out err);

                if (err != IntPtr.Zero)
                {
                    WinPtyException.Throw(err);
                }

                spawnCfg = Native.SpawnConfigNew(Native.SPAWN_FLAG_AUTO_SHUTDOWN, exe, GetArgsString(args), workingDirectory, GetEnvironmentString(environment), out err);

                if (err != IntPtr.Zero)
                {
                    WinPtyException.Throw(err);
                }

                stdin = new StreamWriter(CreatePipe(Native.ConinName(handle), PipeDirection.Out), Encoding.UTF8);
                stdout = new StreamReader(CreatePipe(Native.ConoutName(handle), PipeDirection.In), Encoding.UTF8);
                stderr = stdout;

                DataEvent += OnDataEvent;
                ExitEvent += OnExitEvent;

                BeginOutputReadLine();

                if (!Native.Spawn(handle, spawnCfg, out SafeProcessHandle process, out SafeThreadHandle thread, out uint procError, out err))
                {
                    WinPtyException.Throw(err);
                }

                Pid = Win32.Kernel32.Native.GetProcessId(process);

                Tid = Win32.Kernel32.Native.GetThreadId(thread);

            //}
            //finally
            //{
            //    Native.ConfigFree(cfg);
            //    Native.SpawnConfigFree(spawnCfg);
            //    Native.ErrorFree(err);
            //}
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stdin?.Dispose();
                    stdout?.Dispose();
                    stderr?.Dispose();
                    Native.ConfigFree(cfg);
                    Native.SpawnConfigFree(spawnCfg);
                    Native.ErrorFree(err);
                    Native.Free(handle);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public void OnDataEvent(object sender, WinPtyDataEventArgs e)
        {
            Console.WriteLine(e.Data);
            //WinPtyDataEventHandler winPtyDataEventHandler = DataEvent;
            //if (winPtyDataEventHandler != null)
            //{
            //    winPtyDataEventHandler(this, e);
            //}
        }

        internal void OnDataEventNotify(string data)
        {
            WinPtyDataEventHandler winPtyDataEventHandler = DataEvent;
            if (winPtyDataEventHandler != null)
            {
                WinPtyDataEventArgs e = new WinPtyDataEventArgs(data);
                winPtyDataEventHandler(this, e);
            }
        }

        public void OnExitEvent(object sender, WinPtyExitEventArgs e)
        {
            Exited = true;
            Console.WriteLine(e.ExitCode);

            //WinPtyDataEventHandler winPtyDataEventHandler = DataEvent;
            //if (ExitEvent != null)
            //{
            //    ExitEvent(this, e);
            //}
        }

        internal void OnExitEventNotify(int exitCode, int? signal = null)
        {
            WinPtyExitEventHandler winPtyExitEventHandler = ExitEvent;
            if (winPtyExitEventHandler != null)
            {
                WinPtyExitEventArgs e = new WinPtyExitEventArgs(exitCode, signal);
                winPtyExitEventHandler(this, e);
            }
        }

        public void Resize(int columns, int rows)
        {
            Native.SetSize(handle, Math.Max(columns, 1), Math.Max(rows, 1), out IntPtr err);
        }

        public void Write(string data)
        {
            stdin.Write(data);
        }

        public void WriteLine(string data)
        {
            stdin.WriteLine(data);
        }

        public void Kill(string signal)
        {
            //_inSocket.readable  = false;
            //_inSocket.writable  = false;
            //_outSocket.readable = false;
            //_outSocket.writable = false;
            //// Tell the agent to kill the pty, this releases handles to the process
            //if (_useConpty)
            //{
            //    (_ptyNative as IConptyNative).kill(_pty);
            //}
            //else
            //{
            //    const processList : number[] = (_ptyNative as IWinptyNative).getProcessList(_pid);
            //    (_ptyNative as IWinptyNative).kill(_pid, _innerPidHandle);
            //    // Since pty.kill will kill most processes by itself and process IDs can be
            //    // reused as soon as all handles to them are dropped, we want to immediately
            //    // kill the entire console process list. If we do not force kill all
            //    // processes here, node servers in particular seem to become detached and
            //    // remain running (see Microsoft/vscode#26807).
            //    processList.forEach(pid =>
            //                        {
            //                            try
            //                            {
            //                                process.kill(pid);
            //                            }
            //                            catch (e)
            //                            {
            //                                // Ignore if process cannot be found (kill ESRCH error)
            //                            }
            //                        });
            //}
        }

        public void BeginOutputReadLine()
        {
            if (_output == null)
            {
                Stream s = stdout.BaseStream;
                _output = new AsyncStreamReader(s, OnDataEventNotify, Encoding.UTF8);
            }
            _output.BeginReadLine();
        }

        //private void ListenToStdOut()
        //{
        //    Task.Factory.StartNew(async () =>
        //                          {
        //                              using (var reader = new StreamReader(_stdout))
        //                              {
        //                                  do
        //                                  {
        //                                      var buffer    = new byte[1024];
        //                                      var readChars = await _stdout.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

        //                                      if (readChars > 0)
        //                                      {
        //                                          _terminalsManager.DisplayTerminalOutput(Id, buffer);
        //                                      }
        //                                  }
        //                                  while (!_exited);
        //                              }
        //                          }, TaskCreationOptions.LongRunning);
        //}

        private string GetArgsString(string[] args)
        {
            if (args == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.AppendFormat($"{arg}\0");
            }
            //sb.Append('\0');
            return sb.ToString();
        }

        private string GetEnvironmentString(Dictionary<string, string> environmentVariables)
        {
            if (environmentVariables == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var kvp in environmentVariables)
            {
                sb.AppendFormat("{0}={1}\0", kvp.Key, kvp.Value);
            }
            //sb.Append('\0');
            return sb.ToString();
        }

        private PipeStream CreatePipe(string pipeName, PipeDirection direction)
        {
            string serverName = ".";

            if (pipeName.StartsWith("\\"))
            {
                int slash3 = pipeName.IndexOf('\\', 2);

                if (slash3 != -1)
                {
                    serverName = pipeName.Substring(2, slash3 - 2);
                }

                int slash4 = pipeName.IndexOf('\\', slash3 + 1);

                if (slash4 != -1)
                {
                    pipeName = pipeName.Substring(slash4 + 1);
                }
            }

            var pipe = new NamedPipeClientStream(serverName, pipeName, direction);//, System.IO.Pipes.PipeOptions.Asynchronous);

            pipe.Connect();

            return pipe;
        }

    }

}
