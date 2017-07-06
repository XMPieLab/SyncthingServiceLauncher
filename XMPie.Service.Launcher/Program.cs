using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XMPie.Service.Launcher;

namespace XMPie.Service.Launcher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
//Stop service for debugging by changing while loop
#if DEBUG
            int i = 0;
            while (i++ < 1)
            {
                System.Threading.Thread.Sleep(1000);
            }
#endif
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceEntryClass(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
