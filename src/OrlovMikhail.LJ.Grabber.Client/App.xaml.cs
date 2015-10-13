using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using OrlovMikhail.LJ.Grabber;
using log4net;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            Log.Info("Hello World");
            base.OnStartup(e);
        }
    }
}
