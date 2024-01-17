using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Core.Execution
{
    public interface IWinptyProcess
    {
        int pty { get; set; }
        int fd { get; set; }
        string conin { get; set; }
        string conout { get; set; }
        int pid { get; set; }
        int innerPid { get; set; }
        int innerPidHandle { get; set; }
    }

    public interface IWinptyNative
    {
        IWinptyProcess startProcess(string file, string commandLine, string[] env, string cwd, int cols, int rows, bool debug);
        void resize(int processHandle, int cols, int rows);
        void kill(int pid, int innerPidHandle);
        int[] getProcessList(int pid);
        int getExitCode(int innerPidHandle);
    }

    public class WinPtyAgent
    {
        public Socket inSocket { get; set; }
        public Socket outSocket { get; set; }
        public int pid { get; set; }
        public int innerPid { get; set; }
        public int innerPidHandle { get; set; }
        public Timer _closeTimeout { get; set; }
        public int exitCode { get; set; }

        public int fd { get; set; }
        public int pty { get; set; }
        public IWinptyNative ptyNative { get; set; }


        public WinPtyAgent(string file, string[] args, string[] env, string cwd, int cols, int rows, bool debug, bool useConpty)
        {
            //if (_useConpty === undefined || _useConpty === true)
            //{
            //    _useConpty = _getWindowsBuildNumber() >= 17692;
            //}

            //if (_useConpty)
            //{
            //    if (!conptyNative)
            //    {
            //        conptyNative = loadNative('conpty');
            //    }
            //}
            //else
            //{
            //    if (!winptyNative)
            //    {
            //        winptyNative = loadNative('pty');
            //    }
            //}

            ptyNative = loadNative('pty');



            // Compose command line
            string commandLine = argsToCommandLine(file, args);

            // Open pty session.
            let term: IConptyProcess | IWinptyProcess;
            if (_useConpty)
            {
                term = (_ptyNative as IConptyNative).startProcess(file, cols, rows, debug, _generatePipeName());
            }
            else
            {
                term = (_ptyNative as IWinptyNative).startProcess(file, commandLine, env, cwd, cols, rows, debug);
                _pid = (term as IWinptyProcess).pid;
                _innerPid = (term as IWinptyProcess).innerPid;
                _innerPidHandle = (term as IWinptyProcess).innerPidHandle;
            }

            // Not available on windows.
            _fd = term.fd;

            // Generated incremental number that has no real purpose besides  using it
            // as a terminal id.
            _pty = term.pty;

            // Create terminal pipe IPC channel and forward to a local unix socket.
            _outSocket = new Socket();
            _outSocket.setEncoding('utf8');
            _outSocket.connect(term.conout, () =>
            {
                // TODO: Emit event on agent instead of socket?

                // Emit ready event.
                _outSocket.emit('ready_datapipe');
            });

            _inSocket = new Socket();
            _inSocket.setEncoding('utf8');
            _inSocket.connect(term.conin);
            // TODO: Wait for ready event?

            if (_useConpty)
            {
                const connect = (_ptyNative as IConptyNative).connect(_pty, commandLine, cwd, env, _$onProcessExit.bind(this));
                _innerPid = connect.pid;
            }
        }

        public void resize(cols: number, rows: number)
        {
            if (_useConpty)
            {
                if (_exitCode !== undefined)
                {
                    throw new Error('Cannot resize a pty that has already exited');
                }
                _ptyNative.resize(_pty, cols, rows);
                return;
            }
            _ptyNative.resize(_pid, cols, rows);
        }

        public void kill()
        {
            _inSocket.readable = false;
            _inSocket.writable = false;
            _outSocket.readable = false;
            _outSocket.writable = false;
            // Tell the agent to kill the pty, this releases handles to the process
            if (_useConpty)
            {
                (_ptyNative as IConptyNative).kill(_pty);
            }
            else
            {
                const processList: number[] = (_ptyNative as IWinptyNative).getProcessList(_pid);
                (_ptyNative as IWinptyNative).kill(_pid, _innerPidHandle);
                // Since pty.kill will kill most processes by itself and process IDs can be
                // reused as soon as all handles to them are dropped, we want to immediately
                // kill the entire console process list. If we do not force kill all
                // processes here, node servers in particular seem to become detached and
                // remain running (see Microsoft/vscode#26807).
                processList.forEach(pid =>
                {
                    try
                    {
                        process.kill(pid);
                    }
                    catch (e)
                    {
                        // Ignore if process cannot be found (kill ESRCH error)
                    }
                });
            }
        }

        public int exitCode()
        {
            if (_useConpty)
            {
                return _exitCode;
            }
            return (_ptyNative as IWinptyNative).getExitCode(_innerPidHandle);
        }

        private _getWindowsBuildNumber() : number {
    const osVersion = (/ (\d+)\.(\d+)\.(\d+)/g).exec(os.release());
        let buildNumber: number = 0;
    if (osVersion && osVersion.length === 4) {
      buildNumber = parseInt(osVersion[3]);
    }
    return buildNumber;
  }

private _generatePipeName() : string {
    return `conpty-${Math.random() * 10000000}`;
  }

  /**
   * Triggered from the native side when a contpy process exits.
   */
  private _$onProcessExit(exitCode: number) : void {
    _exitCode = exitCode;
    _flushDataAndCleanUp();
_outSocket.on('data', () => _flushDataAndCleanUp());
  }

  private _flushDataAndCleanUp() : void {
    if (_closeTimeout) {
      clearTimeout(_closeTimeout);
    }
    _closeTimeout = setTimeout(() => _cleanUpProcess(), FLUSH_DATA_INTERVAL);
  }

  private _cleanUpProcess() : void {
    _inSocket.readable = false;
    _inSocket.writable = false;
    _outSocket.readable = false;
    _outSocket.writable = false;
    _outSocket.destroy();
  }
}
    }
}
