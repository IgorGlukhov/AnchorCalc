using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AnchorCalc.Views;
using AnchorCalc.Views.MainWindow;
using Autofac;

namespace AnchorCalc.Bootstrapper
{
    public class Bootstrapper:IDisposable
    {
        private IContainer _container;
        public Bootstrapper()
        {
            var containerBuilder=new ContainerBuilder();
            containerBuilder.RegisterModule<RegistrationModule>().RegisterModule<ViewModels.RegistrationModule>();
            _container=containerBuilder.Build();
        }
        public Window Run()
        {
            var mainWindow=_container.Resolve<IMainWindow>();
            if (mainWindow is not Window window)
            {
                throw new NotImplementedException();
            }
            window.Show();
            return window;        
        }
        public void Dispose()
        {            
            _container.Dispose();
        }
    }
}
