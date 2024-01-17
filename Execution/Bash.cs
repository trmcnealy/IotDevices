using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

using Core.Win32;

namespace Core.Execution
{

    public static class ProcessExtensions
    {
        public static unsafe int ParentProcessId(this Process process)
        {
            using (SafeProcessHandle handle = Kernel32.Native.OpenProcess(ProcessDesiredAccess.PROCESS_QUERY_INFORMATION, false, process.Id))
            {
                if(NtDll.NtQueryInformationProcess(handle,
                                                   PROCESSINFOCLASS.ProcessBasicInformation,
                                                   out PROCESS_BASIC_INFORMATION info,
                                                   (uint)sizeof(PROCESS_BASIC_INFORMATION),
                                                   out _) != 0)
                {
                    throw new Win32Exception("ProcessInformationUnavailable");
                }

                return (int)info.InheritedFromUniqueProcessId;
            }

        }

        public static IReadOnlyList<Process> GetChildProcesses(this Process thisProcess)
        {
            Process[] processes = Process.GetProcesses();

            List<Process> childProcesses = new List<Process>(processes.Length);

            for(int i = 0; i < processes.Length; i++)
            {
                if(processes[i].ParentProcessId() == thisProcess.Id)
                {
                    childProcesses.Add(processes[i]);
                }
            }
            
            return childProcesses;
        }
    }

    //"C:\POSIX\usr\bin\mintty"  -i /msys2.ico -t "MinGW x64" "/usr/bin/bash" --login

    public sealed class Bash : IDisposable
    {

        private static readonly Regex _MatchError = new Regex("error:");
        private static readonly Regex _MatchWarning = new Regex("warning:");
        private static readonly Regex _MatchNote = new Regex("note:");
        private static readonly Regex _MatchFileErrorRegex = new Regex(@"^(.*):(\d+):(\d+):\s+error:(.*)");
        private static readonly Regex _MatchFileWarningRegex = new Regex(@"^(.*):(\d+):(\d+):\s+warning:(.*)");
        private static readonly Regex _MatchFileNoteRegex = new Regex(@"^(.*):(\d+):(\d+):\s+note:(.*)");
        
        private const string CMD = "cmd.exe";

        private const string _minttyPath = @"C:\POSIX\usr\bin\mintty.exe";
        private const string _bashPath = @"C:\POSIX\usr\bin\bash.exe";

        private const string _winpty = "winpty cmd";

        private readonly Process _process;

        private const bool _redirectStandardInput = true;
        private const bool _redirectStandardOutput = true;
        private const bool _redirectStandardError = true;
        private const bool _useShellExecute = false;
        private const bool _createNoWindow = true;
        public bool IsConsole { get; }
        public bool Verbose { get; }
        public TextWriter In { get; }
        public TextWriter Out { get; }
        public TextWriter Error { get; }
        internal ProcessStartInfo StartInfo { get; }

        public Bash(string workingDirectory) //string bashArgs)
                                             //TextWriter outStream = null)
        {
            Verbose = false;

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Out = Console.Out;
            Error = Console.Error;
            IsConsole = true;
            //}





            //C:\POSIX\usr\local\bin\winpty.exe C:\POSIX\usr\bin\mintty.exe -t "MinGW x64" "/usr/bin/bash" --login













            //string userName = Environment.GetEnvironmentVariable("USERNAME");
            StartInfo = new ProcessStartInfo
            {
                FileName = _bashPath,
                //-w hide 
                //Arguments = $"-i /msys2.ico -t \"MinGW x64\" \"/usr/bin/bash\" --login --exec {bash_args}",
                //Arguments = $"-i /msys2.ico -t "MinGW x64" "/usr/bin/bash" --login -c \"{_winpty}\"",
                //Arguments = $"-i /msys2.ico -t \"MinGW x64\" \"/usr/bin/bash\" --login",
                //--exec ./myProgramToExecute.sh
                RedirectStandardInput = _redirectStandardInput,
                RedirectStandardOutput = _redirectStandardOutput,
                RedirectStandardError = _redirectStandardError,
                UseShellExecute = _useShellExecute,
                CreateNoWindow = _createNoWindow,
                WorkingDirectory = workingDirectory
            };

            StartInfo.EnvironmentVariables["HOME"] = "C:/POSIX/home";
            StartInfo.EnvironmentVariables["USERNAME"] = "tehgo";
            StartInfo.EnvironmentVariables["USER"] = "tehgo";
            StartInfo.EnvironmentVariables["CHERE_INVOKING"] = "enabled_from_arguments";
            StartInfo.EnvironmentVariables["CONTITLE"] = "MinGW x64";
            StartInfo.EnvironmentVariables["LOGINSHELL"] = "bash";
            StartInfo.EnvironmentVariables["MINGW64"] = "C:/POSIX/mingw64";
            StartInfo.EnvironmentVariables["MINGW_CHOST"] = "x86_64-w64-mingw32";
            StartInfo.EnvironmentVariables["MINGW_PACKAGE_PREFIX"] = "mingw-w64-x86_64";
            StartInfo.EnvironmentVariables["MINGW_PREFIX"] = "/mingw64";
            StartInfo.EnvironmentVariables["MSYS"] = "winsymlinks:nativestrict";
            StartInfo.EnvironmentVariables["MSYSCON"] = "mintty.exe";
            StartInfo.EnvironmentVariables["MSYSTEM"] = "MINGW64";
            StartInfo.EnvironmentVariables["MSYSTEM_CARCH"] = "x86_64";
            StartInfo.EnvironmentVariables["MSYSTEM_CHOST"] = "x86_64-w64-mingw32";
            StartInfo.EnvironmentVariables["MSYSTEM_PREFIX"] = "/mingw64";
            //startInfo.EnvironmentVariables["SHELL"] = "/usr/bin/bash";
            //startInfo.EnvironmentVariables["PWD"] = "/d/TFS_Sources/Build/Trilinos";
            StartInfo.EnvironmentVariables["WD"] = "C:/POSIX/usr/bin/";

            //string title   = $"{workingDirectory} - MINGW64";
            //string command = $"{string.Join(" ", args)} & pause";

            //string[] cmdArgs = new []{"/c", "start", title, "/wait", _minttyPath, "/c", command};

            _process = new Process()
            {
                StartInfo = StartInfo,
                EnableRaisingEvents = true
            };

            _process.ErrorDataReceived += ProcessError;
            _process.OutputDataReceived += ProcessOutput;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            //_process.WaitForInputIdle();

            //task.SetResult(_process.ExitCode);
        }

        //public async Task<string[]> Execute(Bash bash, string workingDirectory = null)
        //{
        //    StringWriter stringWriter = new StringWriter();

        //    InternalCommand command = new InternalCommand(stringWriter);
        //    await command.Execute(bash.StartInfo, workingDirectory);

        //    return stringWriter.ToString().Split(new[]
        //                                         {
        //                                             '\r', '\n'
        //                                         },
        //                                         StringSplitOptions.RemoveEmptyEntries);
        //}

        public bool IsRunning
        {
            get { return !_process.HasExited; }
        }

        public Task Wait()
        {
            TaskCompletionSource<int> task = new TaskCompletionSource<int>();

            do
            {
                if (!_process.HasExited)
                {
                    // Refresh the current process property values.
                    _process.Refresh();

                    Console.WriteLine();

                    // Display current process statistics.

                    Console.WriteLine($"{_process} -");
                    Console.WriteLine("-------------------------------------");

                    Console.WriteLine($"  Physical memory usage     : {_process.WorkingSet64}");
                    Console.WriteLine($"  Base priority             : {_process.BasePriority}");
                    Console.WriteLine($"  Priority class            : {_process.PriorityClass}");
                    Console.WriteLine($"  User processor time       : {_process.UserProcessorTime}");
                    Console.WriteLine($"  Privileged processor time : {_process.PrivilegedProcessorTime}");
                    Console.WriteLine($"  Total processor time      : {_process.TotalProcessorTime}");
                    Console.WriteLine($"  Paged system memory size  : {_process.PagedSystemMemorySize64}");
                    Console.WriteLine($"  Paged memory size         : {_process.PagedMemorySize64}");

                    // Update the values for the overall peak memory statistics.
                    //peakPagedMem   = _process.PeakPagedMemorySize64;
                    //peakVirtualMem = _process.PeakVirtualMemorySize64;
                    //peakWorkingSet = _process.PeakWorkingSet64;

                    if (_process.Responding)
                    {
                        Console.WriteLine("Status = Running");
                    }
                    else
                    {
                        Console.WriteLine("Status = Not Responding");
                    }
                }
            } while (!_process.WaitForInputIdle(1000));

            while (!_process.Responding && !_process.HasExited)
            {
                Thread.Sleep(100);
                //_process.WaitForInputIdle();
            }

            if (IsRunning)
            {
                //Thread.Sleep(100);
                _process.WaitForInputIdle();
            }

            //task.SetResult(_process.ExitCode);

            return task.Task;
        }

        #region Write

        public void Write(string value)
        {
            _process.StandardInput.Write(value);
        }

        public void Write(char[] buffer, int index, int count)
        {
            _process.StandardInput.WriteAsync(buffer, index, count);
        }

        public void Write(char[] buffer)
        {
            _process.StandardInput.Write(buffer);
        }

        public void Write(char value)
        {
            _process.StandardInput.Write(value);
        }

        public void Write(ReadOnlySpan<char> buffer)
        {
            _process.StandardInput.Write(buffer);
        }

        public Task WriteAsync(char value)
        {
            return _process.StandardInput.WriteAsync(value);
        }

        public Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return _process.StandardInput.WriteAsync(buffer, cancellationToken);
        }

        public Task WriteAsync(string value)
        {
            return _process.StandardInput.WriteAsync(value);
        }

        public Task WriteAsync(char[] buffer, int index, int count)
        {
            return _process.StandardInput.WriteAsync(buffer, index, count);
        }

        public void WriteLine(string value)
        {
            _process.StandardInput.WriteLine(value);
        }

        public void WriteLine(ReadOnlySpan<char> buffer)
        {
            _process.StandardInput.WriteLine(buffer);
        }

        public Task WriteLineAsync()
        {
            return _process.StandardInput.WriteLineAsync();
        }

        public Task WriteLineAsync(char value)
        {
            return _process.StandardInput.WriteLineAsync(value);
        }

        public Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return _process.StandardInput.WriteLineAsync(buffer, index, count);
        }

        public Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return _process.StandardInput.WriteLineAsync(buffer, cancellationToken);
        }

        public Task WriteLineAsync(string value)
        {
            return _process.StandardInput.WriteLineAsync(value);
        }

        #endregion

        private void ProcessError(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                    string outText = e.Data;

                    Match matchReg = _MatchFileErrorRegex.Match(e.Data);

                    if (matchReg.Success)
                    {
                        Out.WriteLine(matchReg.Groups[1].Value, int.Parse(matchReg.Groups[2].Value), int.Parse(matchReg.Groups[3].Value));
                        outText = matchReg.Groups[4].Value;

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        if (_MatchError.Match(e.Data).Success)
                        {
                            Out.WriteLine(outText);
                        }

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        return;
                    }

                    matchReg = _MatchFileWarningRegex.Match(e.Data);

                    if (matchReg.Success)
                    {
                        Out.WriteLine(matchReg.Groups[1].Value, int.Parse(matchReg.Groups[2].Value), int.Parse(matchReg.Groups[3].Value));
                        outText = matchReg.Groups[4].Value;

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        if (_MatchWarning.Match(e.Data).Success)
                        {
                            Out.WriteLine(outText);
                        }

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        return;
                    }

                    matchReg = _MatchFileNoteRegex.Match(e.Data);

                    if (matchReg.Success)
                    {
                        Out.WriteLine(matchReg.Groups[1].Value, int.Parse(matchReg.Groups[2].Value), int.Parse(matchReg.Groups[3].Value));
                        outText = matchReg.Groups[4].Value;

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        if (_MatchNote.Match(e.Data).Success)
                        {
                            Out.WriteLine(outText);
                        }

                        if (IsConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        return;
                    }

                    if (IsConsole)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Out.WriteLine(outText);

                    if (IsConsole)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.WriteLine(ex.ToString());
            }
        }

        private void ProcessOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Out.WriteLine(e.Data);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            if (_process != null)
            {
                _process.Close();
                _process.Dispose();
            }
        }

        #endregion
    }

}
