using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public class DIContainer
    {
        private IContainer _container;

        public DIContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            if(ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                builder.RegisterType<DesignMainWindowVM>().As<IMainWindowVM>();
            }
            else
            {
                GrabberContainerHelper.RegisterDefaultClasses(builder);

                // Create run time view services and models
                builder.RegisterType<MainWindowVM>().As<IMainWindowVM>();
            }

            _container = builder.Build();
        }


        public IMainWindowVM Main
        {
            get
            {
                return _container.Resolve<IMainWindowVM>();
            }
        }
    }
}
