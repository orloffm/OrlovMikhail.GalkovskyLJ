using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.LJ.Grabber;
using Autofac;
using OrlovMikhail.LJ.BookWriter;
using OrlovMikhail.LJ.BookWriter.AsciiDoc;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            // Configuration.
            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);
            if(!SettingsTools.LoadValue("root", argsDic, Settings.Default, s => s.RootFolder))
                return;
            if(!SettingsTools.LoadValue("source", argsDic, Settings.Default, s => s.Source))
                return;
            Settings.Default.Save();
            bool overWrite = argsDic.ContainsKey("overwrite");

            // Concrete classes.
            ContainerBuilder builder = new ContainerBuilder();
            GrabberContainerHelper.RegisterDefaultClasses(builder);
            BookWriterContainerHelper.RegisterDefaultClasses(builder);
            builder.RegisterType<AsciiDocBookWriterFactory>().As<IBookWriterFactory>();
            builder.RegisterType<BookMaker>().As<IBookMaker>();
            IContainer container = builder.Build();

            // Load all dump files.
            IFileSystem fs = container.Resolve<IFileSystem>();
            DirectoryInfoBase root = fs.DirectoryInfo.FromDirectoryName(Settings.Default.RootFolder);
            FileInfoBase[] dumps = FindAllDumps(Settings.Default.Source, fs);
            log.Info("Dumps found: " + dumps.Length);
            if(dumps.Length == 0)
                return;

            // Run.
            IBookMaker bm = container.Resolve<IBookMaker>();
            bm.Make(root, dumps, overWrite).Wait();
        }

        private static FileInfoBase[] FindAllDumps(string passed, IFileSystem fs)
        {
            FileInfoBase sourceFI = fs.FileInfo.FromFileName(passed);
            if(sourceFI.Exists)
                return new FileInfoBase[] { sourceFI };

            DirectoryInfoBase sourceDI = fs.DirectoryInfo.FromDirectoryName(passed);
            if(sourceDI.Exists)
            {
                FileInfoBase[] found = sourceDI.GetFiles("dump.xml", SearchOption.AllDirectories);
                return found;
            }

            return new FileInfoBase[0];
        }
    }
}
