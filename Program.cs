using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Press Enter to start...");
        Console.ReadLine();

        CleanFiles();
        ResetWindowsStoreCache();
        CleanWindowsUpdateFiles();
        Console.WriteLine("Operation completed.");
    }

    static void CleanFiles()
    {
        try
        {
            
            string tempDir = Path.GetTempPath(); 
            string windowsTempDir = @"C:\Windows\Temp"; 

            // Clean user temp directory
            Console.WriteLine("Cleaning user temp directory...");
            DeleteFilesInDirectory(tempDir);

           
            Console.WriteLine("Cleaning Windows temp directory...");
            DeleteFilesInDirectory(windowsTempDir);

            Console.WriteLine("Temporary files cleaned.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void CleanWindowsUpdateFiles()
    {
        try
        {
            
            Console.WriteLine("Stopping Windows Update services...");
            ExecuteCommand("net stop wuauserv");
            ExecuteCommand("net stop cryptSvc");
            ExecuteCommand("net stop bits");
            ExecuteCommand("net stop msiserver");

          
            string downloadDir = @"C:\Windows\SoftwareDistribution\Download";
            Console.WriteLine("Cleaning Windows Update directory...");
            DeleteFilesInDirectory(downloadDir);

           
            Console.WriteLine("Starting Windows Update services...");
            ExecuteCommand("net start wuauserv");
            ExecuteCommand("net start cryptSvc");
            ExecuteCommand("net start bits");
            ExecuteCommand("net start msiserver");

            Console.WriteLine("Windows Update files cleaned.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ResetWindowsStoreCache()
    {
        try
        {
            Console.WriteLine("Resetting Windows Store cache...");
            ExecuteCommand("wsreset.exe");

            // Wait for the cache reset to complete
            Thread.Sleep(10000); // Wait 10 seconds

            // Kill Windows Store if it is still running
            Console.WriteLine("Killing Windows Store if it's still running...");
            ExecuteCommand("taskkill /IM WinStore.App.exe /F");

            Console.WriteLine("Windows Store cache reset and Windows Store killed if it was running.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while resetting Windows Store cache: {ex.Message}");
        }
    }

    static void DeleteFilesInDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
             
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted file: {file}");
                    }
                    catch (IOException ioEx)
                    {
                        Console.WriteLine($"File in use, could not delete: {file}. Error: {ioEx.Message}");
                    }
                }

                
                foreach (string dir in Directory.GetDirectories(path))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        Console.WriteLine($"Deleted directory: {dir}");
                    }
                    catch (IOException ioEx)
                    {
                        Console.WriteLine($"Directory in use, could not delete: {dir}. Error: {ioEx.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {path}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting files in {path}: {ex.Message}");
        }
    }

    static void ExecuteCommand(string command)
    {
        try
        {
            var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }

            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while executing command '{command}': {ex.Message}");
        }
    }
}
