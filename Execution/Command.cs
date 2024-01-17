using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Execution
{

    public static class Command
    {
        private const int MaxPath = 255;

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", PreserveSig = true, CharSet = CharSet.Auto)]
        private static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);

        public static string GetShortPath(string path)
        {
            StringBuilder shortPath = new StringBuilder(MaxPath);
            GetShortPathName(path, shortPath, MaxPath);
            return shortPath.ToString();
        }

        public static async void ExecuteConsole(string exePath, IEnumerable<string> arguments, string? workingDirectory = null)
        {
            InternalCommand command = new InternalCommand();
            await command.Execute(exePath, arguments, workingDirectory);
        }

        public static async void ExecuteConsole(string exePath, IEnumerable<string> arguments, bool verbose, string? workingDirectory = null)
        {
            InternalCommand command = new InternalCommand(verbose);
            await command.Execute(exePath, arguments, workingDirectory);
        }

        public static async void ExecuteConsole(ProcessStartInfo startInfo, string? workingDirectory = null)
        {
            InternalCommand command = new InternalCommand();
            await command.Execute(startInfo, workingDirectory);
        }

        public static async void ExecuteConsole(ProcessStartInfo startInfo, bool verbose, string? workingDirectory = null)
        {
            InternalCommand command = new InternalCommand(verbose);
            await command.Execute(startInfo, workingDirectory);
        }

        public static async Task<string[]> Execute(string exePath, IEnumerable<string> arguments, string? workingDirectory = null)
        {
            StringWriter stringWriter = new StringWriter();

            InternalCommand command = new InternalCommand(stringWriter);
            await command.Execute(exePath, arguments, workingDirectory);

            return stringWriter.ToString().Split(new[]
                                                 {
                                                     '\r', '\n'
                                                 },
                                                 StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<string[]> Execute(string exePath, IEnumerable<string> arguments, bool verbose, string? workingDirectory = null)
        {
            StringWriter stringWriter = new StringWriter();

            InternalCommand command = new InternalCommand(verbose, stringWriter);
            await command.Execute(exePath, arguments, workingDirectory);

            return stringWriter.ToString().Split(new[]
                                                 {
                                                     '\r', '\n'
                                                 },
                                                 StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<string[]> Execute(ProcessStartInfo startInfo, string? workingDirectory = null)
        {
            StringWriter stringWriter = new StringWriter();

            InternalCommand command = new InternalCommand(stringWriter);
            await command.Execute(startInfo, workingDirectory);

            return stringWriter.ToString().Split(new[]
                                                 {
                                                     '\r', '\n'
                                                 },
                                                 StringSplitOptions.RemoveEmptyEntries);
        }

        //public static async Task<string[]> Execute(Bash bash, string? workingDirectory = null)
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

        public static async Task<string[]> Execute(ProcessStartInfo startInfo, bool verbose, string? workingDirectory = null)
        {
            StringWriter stringWriter = new StringWriter();

            InternalCommand command = new InternalCommand(verbose, stringWriter);
            await command.Execute(startInfo, workingDirectory);

            return stringWriter.ToString().Split(new[]
                                                 {
                                                     '\r', '\n'
                                                 },
                                                 StringSplitOptions.RemoveEmptyEntries);
        }
    }

    internal sealed class InternalCommand
    {
        private static readonly Regex _MatchError = new Regex("error:");
        private static readonly Regex _MatchWarning = new Regex("warning:");
        private static readonly Regex _MatchNote = new Regex("note:");

        // E:/Code/Microsoft DirectX SDK (June 2010)//include/xaudio2fx.h:68:1: error:
        private static readonly Regex _MatchFileErrorRegex = new Regex(@"^(.*):(\d+):(\d+):\s+error:(.*)");
        private static readonly Regex _MatchFileWarningRegex = new Regex(@"^(.*):(\d+):(\d+):\s+warning:(.*)");
        private static readonly Regex _MatchFileNoteRegex = new Regex(@"^(.*):(\d+):(\d+):\s+note:(.*)");
        public TextWriter Out { get; }
        public TextWriter Error { get; }
        public bool IsConsole { get; }
        public bool Verbose { get; }

        public InternalCommand(TextWriter? writer = null)
        {
            Verbose = false;

            if (writer == null)
            {
                Out = Console.Out;
                Error = Console.Error;
                IsConsole = true;
            }
            else
            {
                Out = writer;
                Error = writer;
                IsConsole = false;
            }
        }

        public InternalCommand(bool verbose, TextWriter? writer = null)
        {
            Verbose = verbose;

            if (writer == null)
            {
                Out = Console.Out;
                Error = Console.Error;
                IsConsole = true;
            }
            else
            {
                Out = writer;
                Error = writer;
                IsConsole = false;
            }
        }

        public async Task Execute(string exePath, IEnumerable<string> arguments, string? workingDirectory = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = string.Join(" ", arguments),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            await Execute(startInfo, workingDirectory);
        }

        public Task<int> Execute(ProcessStartInfo startInfo, string? workingDirectory = null)
        {
            TaskCompletionSource<int> task = new TaskCompletionSource<int>();

            Process currentProcess = new Process();
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError  = true;
            //startInfo.UseShellExecute        = false;
            //startInfo.CreateNoWindow         = true;
            startInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;

            if (Verbose)
            {
                Debug.WriteLine(startInfo.FileName + " " + startInfo.Arguments);
            }

            currentProcess.StartInfo = startInfo;
            currentProcess.EnableRaisingEvents = true;

            if (startInfo.RedirectStandardOutput)
            {
                currentProcess.OutputDataReceived += ProcessOutputFromHeaderFile; //(s, e) => Out.WriteLine(e.Data);
            }

            if (startInfo.RedirectStandardError)
            {
                currentProcess.ErrorDataReceived += ProcessErrorFromHeaderFile;
            }

            currentProcess.Start();

            if (startInfo.RedirectStandardOutput)
            {
                currentProcess.BeginOutputReadLine();
            }

            if (startInfo.RedirectStandardError)
            {
                currentProcess.BeginErrorReadLine();
            }

            currentProcess.WaitForExit();

            task.SetResult(currentProcess.ExitCode);

            currentProcess.Close();

            return task.Task;
        }

        /// <summary>
        ///     Processes the error from header file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Diagnostics.DataReceivedEventArgs" /> instance containing the event data.</param>
        private void ProcessErrorFromHeaderFile(object sender, DataReceivedEventArgs e)
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

        private void ProcessOutputFromHeaderFile(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Out.WriteLine(e.Data);
            }
        }
    }

}
