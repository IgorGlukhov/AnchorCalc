using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AnchorCalc.Infrastructure.Settings;
using AnchorCalc.ViewModels;
using AnchorCalc.Views.MainWindow;
using Autofac;

namespace AnchorCalc.Bootstrapper
{
    public class Bootstrapper:IDisposable
    {
        private readonly IContainer _container;
        public Bootstrapper()
        {
            var containerBuilder=new ContainerBuilder();
            containerBuilder
                .RegisterModule<Infrastructure.RegistrationModule>()
                .RegisterModule<Views.RegistrationModule>()
                .RegisterModule<RegistrationModule>(); 
            _container=containerBuilder.Build();
        }
        public Window Run()
        {
            InitializeDependencies();
            var mainWindow=_container.Resolve<IMainWindow>();
            if (mainWindow is not Window window)
            {
                throw new NotImplementedException();
            }
            window.Show();
            return window;        
        }

        private void InitializeDependencies()
        {
            _container.Resolve<IMainWindowMementoWrapperInitializer>().Initialize();
        }

        public void Dispose()
        {            
            _container.Dispose();
        }
    }
}
