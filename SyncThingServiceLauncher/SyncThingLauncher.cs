using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XMPie.Assembly.Utils;

namespace SyncThingServiceLauncher
{
    public class SyncThingLauncher: IServiceBase
    {
        private static Process SyncThingProc;
        private static bool StopCalled = false;
        private const int UPGRADE_SLEEP_DURATION =5000;
        private const string GUI_LOG_ENTRY = "access the gui via the following url:";
        private string outPutFile = "syncthingOutput.txt";

        public void Start(string[] args)
        {
            Task task = Task.Run(() =>
            {
                while (!StopCalled)
                {
                    StartSyncthingProcess();
                    SyncThingProc.WaitForExit();
                    HandleExitCodes();
                }
            });
        }

        private static void HandleExitCodes()
        {
            if (SyncThingProc.ExitCode == (int) ExitCodes.Restarting ||
                SyncThingProc.ExitCode == (int) ExitCodes.Upgrading)
            {
                System.Threading.Thread.Sleep(UPGRADE_SLEEP_DURATION);
                //Make sure that only single process exists by waiting for upgrade to complete
                //Then kill new process that went up and perform a controlled restart of service
                foreach (var process in Process.GetProcessesByName("syncthing.exe"))
                {
                    process.Kill();
                }
            }
        }

        private void StartSyncthingProcess()
        {
            SyncThingProc = new Process();
            ProcessStartInfo startInfo = GetProcessStartInfo();
            SyncThingProc.StartInfo = startInfo;
            SyncThingProc.Start();
            SyncThingProc.ErrorDataReceived += SyncThingProc_ErrorDataReceived;
            SyncThingProc.OutputDataReceived += SyncThingProc_OutputDataReceived;
            //Open 2 threads for async logging of error and output pipes
            SyncThingProc.BeginOutputReadLine();
            SyncThingProc.BeginErrorReadLine();
        }

        private static ProcessStartInfo GetProcessStartInfo()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = "-no-restart -no-browser";
            startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syncthing.exe");
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            return startInfo;
        }


        private void SyncThingProc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (outPutFile)
            {
                string logFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outPutFile);
               
                using (StreamWriter sw = System.IO.File.AppendText(logFullPath))
                {
                    using (StringReader sr = new StringReader(e.Data))
                    {
                        string line = sr.ReadLine();
                        WriteShortCutToSyncthing(line);
                        sw.WriteLine(line);
                    }
                }
                BackupOutputOnFileTooBig(logFullPath);
            }
        }

        private void BackupOutputOnFileTooBig(string logFullPath)
        {
            string backupFullPath = logFullPath + "_old";
            long fileLength = new System.IO.FileInfo(logFullPath).Length;
            if (fileLength > 0x100000)
            {
                if (File.Exists(backupFullPath))
                {
                    File.Delete(backupFullPath);
                }
                File.Move(logFullPath, backupFullPath);
            }
        }

        private void WriteShortCutToSyncthing(string line)
        {
            if (line.ToLower().IndexOf(GUI_LOG_ENTRY) > -1)
            {
                string url = line.Substring(line.ToLower().IndexOf(GUI_LOG_ENTRY) + GUI_LOG_ENTRY.Length + 1);
                urlShortcut(url);
            }
        }

        private void urlShortcut(string linkUrl)
        {
            string syncthingUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "manageSyncthing.url");
            try
            {
                using (StreamWriter writer = new StreamWriter(File.Create(syncthingUrl)))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=" + linkUrl);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                //Just ignore failure
            }
        }

        private void SyncThingProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errlog.txt")))
            {
                sw.WriteLine(e.Data);
            }
        }

        public void Stop(string[] args)
        {
            StopCalled = true;
            try
            {
                if(!SyncThingProc.HasExited) SyncThingProc.Kill();
            }
            catch (Exception)
            {
                
                //throw;
            }
            
        }
    }
}
