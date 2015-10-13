using Autofac;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class GrabberContainerHelper
    {
        public static void RegisterDefaultClasses(ContainerBuilder builder)
        {
            builder.RegisterType<LJClient>().As<ILJClient>().SingleInstance();
            builder.RegisterType<Worker>().As<IWorker>().SingleInstance();

            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<FileStorage>().As<IFileStorage>();
            builder.RegisterType<FileStorageFactory>().As<IFileStorageFactory>();
            builder.RegisterType<FileUrlExtractor>().As<IFileUrlExtractor>();
            builder.RegisterType<UserpicStorageFactory>().As<IUserpicStorageFactory>();
            builder.RegisterType<RelatedDataSaver>().As<IRelatedDataSaver>();
            builder.RegisterType<SuitableCommentsPicker>().As<ISuitableCommentsPicker>();

            builder.RegisterType<OtherPagesLoader>().As<IOtherPagesLoader>();
            builder.RegisterType<EntryBaseHelper>().As<IEntryBaseHelper>();
            builder.RegisterType<EntryHelper>().As<IEntryHelper>();
            builder.RegisterType<EntryPageHelper>().As<IEntryPageHelper>();
            builder.RegisterType<RepliesHelper>().As<IRepliesHelper>();
            builder.RegisterType<LayerParser>().As<ILayerParser>();
            builder.RegisterType<Extractor>().As<IExtractor>();
        }
    }
}
