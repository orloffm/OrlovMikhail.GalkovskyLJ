using Autofac;
using OrlovMikhail.LJ.Grabber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using log4net;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky.Dumper
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            ContainerBuilder builder = new ContainerBuilder();
            GrabberContainerHelper.RegisterDefaultClasses(builder);
            IContainer _container = builder.Build();

            IWorker w = _container.Resolve<IWorker>();
            INumberingStrategy gsg = new GalkovskyNumberingStrategy();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);

            if (!SettingsTools.LoadValue("url", argsDic, Settings.Default, s => s.LatestUrl))
                return;
            if (!SettingsTools.LoadValue("root", argsDic, Settings.Default, s => s.RootFolder))
                return;
            if (!SettingsTools.LoadValue("cookie", argsDic, Settings.Default, s => s.Cookie))
                return;

            bool continueWithNext = argsDic.ContainsKey("continue");

            while (true)
            {
                Console.WriteLine("==============================");
                Console.WriteLine(Settings.Default.LatestUrl);
                Console.WriteLine("==============================");
                Console.WriteLine();

                try
                {
                    Settings.Default.Save();
                    EntryPage result = w.Work(Settings.Default.LatestUrl, Settings.Default.RootFolder, gsg, Settings.Default.Cookie);
                    string nextUrl = result.Entry.NextUrl;
                    if(!String.IsNullOrWhiteSpace(nextUrl))
                        Settings.Default.LatestUrl = nextUrl;
                    else
                        continueWithNext = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("==============================");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("==============================");
                    Console.Read();
                    return;
                }

                if (!continueWithNext)
                {
                    Console.WriteLine("Exiting.");
                    return;
                }
                else
                {
                    Console.WriteLine("Will autocontinue.");
                    Console.WriteLine();
                }
            }
        }


    }
}
