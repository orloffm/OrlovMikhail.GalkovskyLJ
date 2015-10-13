using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.BookWriter
{
    public static class BookWriterContainerHelper
    {
        public static void RegisterDefaultClasses(ContainerBuilder builder)
        {
            builder.RegisterType<HTMLParser>().As<IHTMLParser>();
            builder.RegisterType<PostPartsMaker>().As<IPostPartsMaker>();
        }
    }
}
