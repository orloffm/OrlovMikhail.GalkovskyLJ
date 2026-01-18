using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Serilog;
using Autofac;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.LJ.BookWriter;
using OrlovMikhail.LJ.BookWriter.AsciiDoc;
using OrlovMikhail.LJ.Galkovsky.Tools;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            // Configuration.
            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);

            if (!argsDic.TryGetValue("root", out string rootFolder) || string.IsNullOrWhiteSpace(rootFolder))
            {
                Log.Error("Missing required argument: -root");
                PrintUsage();
                return;
            }

            if (!argsDic.TryGetValue("source", out string source) || string.IsNullOrWhiteSpace(source))
            {
                Log.Error("Missing required argument: -source");
                PrintUsage();
                return;
            }

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
            IDirectoryInfo root = fs.DirectoryInfo.New(rootFolder);
            IFileInfo[] dumps = FindAllDumps(source, fs);
            Log.Information("Dumps found: {Count}", dumps.Length);
            if (dumps.Length == 0)
                return;

            // Run.
            IBookMaker bm = container.Resolve<IBookMaker>();
            bm.Make(root, dumps, overWrite).Wait();
        }

        private static IFileInfo[] FindAllDumps(string passed, IFileSystem fs)
        {
            IFileInfo sourceFI = fs.FileInfo.New(passed);
            if (sourceFI.Exists)
                return new IFileInfo[] { sourceFI };

            IDirectoryInfo sourceDI = fs.DirectoryInfo.New(passed);
            if (sourceDI.Exists)
            {
                IFileInfo[] found = sourceDI.GetFiles("dump.xml", System.IO.SearchOption.AllDirectories);
                return found;
            }

            return new IFileInfo[0];
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: bookmaker -root <folder> -source <path> [-overwrite]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  -root       Root folder for output");
            Console.WriteLine("  -source     Source dump.xml file or folder containing dumps");
            Console.WriteLine("  -overwrite  Overwrite existing fragment files");
        }
    }
}
