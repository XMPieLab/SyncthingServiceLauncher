using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XMPie.Assembly.Utils;

namespace XMPie.Service.Launcher
{
    public partial class ServiceEntryClass : ServiceBase
    {
        private string pModuleName;
        private IServiceBase pLoadedModule;
        public ServiceEntryClass(string[] args)
        {
            if (args == null || args.Length < 2) return;
            string ModuleName = args[1];
            pModuleName = ModuleName;
            //int delay = 5;
            //while (delay-- > 0)
            //{
            //    System.Threading.Thread.Sleep(1000);
            //}
  pLoadedModule= Factory.GetInstances<IServiceBase>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pModuleName + ".dll")).FirstOrDefault();
            InitializeComponent();
            
        }
        protected override void OnStart(string[] args)
        {
            pLoadedModule?.Start(args);

        }
        protected override void OnStop()
        {
            pLoadedModule?.Stop(null);
        }
    }
}
