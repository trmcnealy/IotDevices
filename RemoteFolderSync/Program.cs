
using System;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Runtime.Intrinsics.X86;
using System.Security;

using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Security;


namespace RemoteFolderSync;

internal class Program
{

    static string? pathToMonitor;
    static string? destinationServer;
    static string? destinationPath;
    static string? username;
    static SecureString? password;


    //E:/Github/trmcnealy/IotDevices/RaspberryPiDevices.Tests/bin/publish trmpi ~/Projects trmcnealy C@Mero406420
    static void Main(string[] args)
    {
        if (args.Length != 5)
        {
            throw new Exception("{pathToMonitor} {destinationServer} {destinationPath} {username} {password}");
        }

        pathToMonitor = args[0];
        destinationServer = args[1];
        destinationPath = args[2];
        username = args[3];
        password = new SecureString();
        foreach (char ch in args[4])
        {
            password.AppendChar(ch);
        }

        using FileSystemWatcher watcher = new FileSystemWatcher(pathToMonitor);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;

        //watcher.Filter = "*.txt";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

    private static void OnChanged(object sender, FileSystemEventArgs eventArgs)
    {
        if (eventArgs.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        Console.WriteLine($"Changed: {eventArgs.FullPath}");

        SyncFile(eventArgs.FullPath);


        //List<string> arguments = new List<string>();

        //arguments.Add($"-r");
        //arguments.Add($"-B");
        //arguments.Add($"{password}");
        //arguments.Add($"E:/Github/trmcnealy/IotDevices/RaspberryPiDevices.Tests/bin/publish/*");
        //arguments.Add($"trmcnealy@trmpi:~/Projects");


        //Execution.Command.ExecuteConsole("scp", arguments);





        ////The key fingerprint is:
        ////SHA256:9yUJO7opZtWVHc40zUH6ncw/C//4cmroPu/Ahe26hAo trmcnealy@trmpi
        ////The key's randomart image is:
        ////+--[ED25519 256]--+
        ////|              .=.|
        ////|              = o|
        ////|          .  B o |
        ////|           oo+B o|
        ////|        S.+.+ +=.|
        ////|        .o.= =  .|
        ////|      E.. . =o...|
        ////|      +. + .oo=.+|
        ////|     o .+  o=*=B+|
        ////+----[SHA256]-----+
        //string expectedFingerPrint = "SHA256:9yUJO7opZtWVHc40zUH6ncw/C//4cmroPu/Ahe26hAo trmcnealy@trmpi";

        //using (SshClient client = new SshClient(destinationServer, username, password.ToString()))
        //{
        //    client.HostKeyReceived += (sender, e) =>
        //    {
        //        e.CanTrust = expectedFingerPrint.Equals(e.FingerPrintSHA256);
        //    };
        //    client.Connect();
        //    client.RunCommand("ls");
        //    client.RunCommand("");
        //}
    }

    private static void OnCreated(object sender, FileSystemEventArgs eventArgs)
    {
        Console.WriteLine($"Changed: {eventArgs.FullPath}");
        SyncFile(eventArgs.FullPath);
    }

    private static void SyncFile(string filepath)
    {
        ConnectionInfo connectionInfo = new ConnectionInfo(destinationServer, username,
                                        new PasswordAuthenticationMethod(username, password?.ToString()),
                                        new PrivateKeyAuthenticationMethod("ED25519"));

        using (SftpClient client = new SftpClient(connectionInfo))
        {
            client.Connect();

            if (client.Exists(destinationPath))
            {
                client.DeleteFile(destinationPath);
            }

            string remoteFilePath = destinationPath + '/' + Path.GetFileName(filepath).Replace('\\', '/');

            Console.WriteLine($"remoteFilePath: {remoteFilePath}");

            try
            {
                client.WriteAllBytes(remoteFilePath, File.ReadAllBytes(filepath));
            }
            catch (SftpPathNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }



    //private static void OnDeleted(object sender, FileSystemEventArgs e)
    //{
    //    Console.WriteLine($"Deleted: {e.FullPath}");
    //}

    //private static void OnRenamed(object sender, RenamedEventArgs e)
    //{
    //    Console.WriteLine($"Renamed:");
    //    Console.WriteLine($"    Old: {e.OldFullPath}");
    //    Console.WriteLine($"    New: {e.FullPath}");
    //}

    //private static void OnError(object sender, ErrorEventArgs e)
    //{
    //    PrintException(e.GetException());
    //}

    //private static void PrintException(Exception? ex)
    //{
    //    if (ex != null)
    //    {
    //        Console.WriteLine($"Message: {ex.Message}");
    //        Console.WriteLine("Stacktrace:");
    //        Console.WriteLine(ex.StackTrace);
    //        Console.WriteLine();
    //        PrintException(ex.InnerException);
    //    }
    //}
}
