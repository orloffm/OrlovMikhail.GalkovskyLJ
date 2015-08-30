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

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);

            if (!SettingsTools.LoadValue("root", argsDic, Settings.Default, s => s.RootFolder))
                return;
            if (!SettingsTools.LoadValue("source", argsDic, Settings.Default, s => s.Source))
                return;
            Settings.Default.Save();

            bool overWrite = argsDic.ContainsKey("overwrite");

            ContainerBuilder builder = new ContainerBuilder();
            GrabberContainerHelper.RegisterDefaultClasses(builder);
            BookWriterContainerHelper.RegisterDefaultClasses(builder);

            builder.RegisterType<AsciiDocBookWriterFactory>().As<IBookWriterFactory>();
            builder.RegisterType<BookMaker>().As<IBookMaker>();

            IContainer container = builder.Build();
            IBookMaker bm = container.Resolve<IBookMaker>();
            IFileSystem fs = container.Resolve<IFileSystem>();

            DirectoryInfoBase root = fs.DirectoryInfo.FromDirectoryName(Settings.Default.RootFolder);
            FileInfoBase sourceFI = fs.FileInfo.FromFileName(Settings.Default.Source);
            FileInfoBase targetFI = fs.FileInfo.FromFileName(fs.Path.Combine(sourceFI.Directory.FullName, "fragment.asc"));

            if (targetFI.Exists && !overWrite)
            {
                // We won't overwrite files if not specifically asked to.
                log.InfoFormat("File {0} already exists, no /overwrite parameter specified, won't overwrite.", targetFI.Name);
                return;
            }

            bm.Make(root, sourceFI, targetFI);
        }
    }
}
