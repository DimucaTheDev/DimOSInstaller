using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace install_DimOS
{
    class Program
    {
        static bool r, r2 = false;

        static void Main(string[] args)
        {
            int megabytes(long bytes) => Convert.ToInt32(bytes) / 1048576;
            const string start = "This program will install and setup DimOS for use,\ndo You want to continue? [y/n]:";
            const string download = "Downloading files...";
            const string latest = "https://dimucathedev.github.io/DimOS/latest.txt";
            string vers = new WebClient().DownloadString(latest);

            Console.Write(start);
            if (Console.ReadKey().Key == ConsoleKey.N)
                Environment.Exit(-1);
            else
            Console.Clear();

            WebClient w = new WebClient();
            WebClient w2 = new WebClient();


            string up = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (!File.Exists(up + "\\DimOS.iso"))
                new Thread(() => w.DownloadFileAsync(new Uri($"https://dimucathedev.github.io/projects/files/DimOS/DimOS_{vers}.iso"),
                    $"{up}\\DimOS.iso")).Start();
            else r = true;
            new Thread(() => w2.DownloadFileAsync(new Uri($"https://dimucathedev.github.io/projects/files/DimOS/Workstation.zip"),
                $"{up}\\ws.zip")).Start();


            w.DownloadProgressChanged += W_DownloadProgressChanged;
            w.DownloadFileCompleted += W_DownloadFileCompleted;

            w2.DownloadProgressChanged += W2_DownloadProgressChanged;
            w2.DownloadFileCompleted += W2_DownloadFileCompleted;

            while (!r || !r2) 
            {
                ;
                ;
                ;
            }
            
            

            Console.Clear();
            Console.WriteLine("Select the location where the VM will be located:");
            string location = Console.ReadLine();
            Console.WriteLine("Setupping...");

            ZipFile.ExtractToDirectory($"{up}\\ws.zip", $"{location}\\DimOS");
            File.Delete(up+"\\ws.zip");
            File.Move(up + "\\DimOS.iso", location+"\\DimOS\\Workstation\\CosmosKernel.iso");
            Console.Clear();
            Console.WriteLine("Setup completed succesfully, before start make sure you have installed VMware.\n" +
                "Press R to start virtual machine");
            if (Console.ReadKey().Key == ConsoleKey.R)
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    FileName = "cmd",
                    Arguments = "/C start " + location + "\\DimOS\\Workstation\\Cosmos.vmx"
                });
        }

        private static void W2_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Config files downloaded                              \n                          ");
            r2 = true;
        }

        private static void W2_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int megabytes(long bytes) => Convert.ToInt32(bytes) / 1048576;
            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"{e.ProgressPercentage}% completed ({megabytes(e.BytesReceived)}/{megabytes(e.TotalBytesToReceive)} MB)");
            Console.SetCursorPosition(0, 1);
        }

        private static void W_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("ISO file downloaded                  \n                                      ");
            r = true;
        }

        private static void W_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int megabytes(long bytes) => Convert.ToInt32(bytes) / 1048576;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"{e.ProgressPercentage}% completed ({megabytes(e.BytesReceived)}/{megabytes(e.TotalBytesToReceive)} MB)");
            Console.SetCursorPosition(0, 0);
        }
    }
}
