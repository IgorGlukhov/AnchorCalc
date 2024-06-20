using System.Windows;
using System.Windows.Threading;
using AnchorCalc.Bootstrapper;

namespace AnchorCalc
{
    public partial class App
    {
        private Bootstrapper.Bootstrapper? _bootstrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _bootstrapper = new Bootstrapper.Bootstrapper();
            MainWindow = _bootstrapper.Run();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            if (_bootstrapper != null) _bootstrapper.Dispose();
            base.OnExit(e);
        }
    }
}
