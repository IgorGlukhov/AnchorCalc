using AnchorCalc.Domain.Settings;
using Newtonsoft.Json;

namespace AnchorCalc.Infrastructure.Settings;

internal class MainWindowMementoWrapper : IMainWindowMementoWrapperInitializer, IMainWindowMementoWrapper, IDisposable
{
    private MainWindowMemento _mainWindowMemento;
    private bool _initialized;
    private string _settingsFilePath;

    public MainWindowMementoWrapper()
    {
        _mainWindowMemento = new MainWindowMemento();
    }

    public void Dispose()
    {
        EnsureInitilized();
        var serializedMemento=JsonConvert.SerializeObject(_mainWindowMemento);
        File.WriteAllText(_settingsFilePath,serializedMemento);
    }

    public double Left
    {
        get
        {
            EnsureInitilized();
            return _mainWindowMemento.Left;
        }
        set
        {
            EnsureInitilized();
            _mainWindowMemento.Left = value;
        }
    }

    public double Top
    {
        get
        {
            EnsureInitilized();
            return _mainWindowMemento.Top;
        }
        set
        {
            EnsureInitilized();
            _mainWindowMemento.Top = value;
        }
    }

    public double Width
    {
        get
        {
            EnsureInitilized();
            return _mainWindowMemento.Width;
        }
        set
        {
            EnsureInitilized();
            _mainWindowMemento.Width = value;
        }
    }

    public double Height
    {
        get
        {
            EnsureInitilized();
            return _mainWindowMemento.Height;
        }
        set
        {
            EnsureInitilized();
            _mainWindowMemento.Height = value;
        }
    }

    public bool IsMaximized
    {
        get
        {
            EnsureInitilized();
            return _mainWindowMemento.IsMaximized;
        }
        set
        {
            EnsureInitilized();
            _mainWindowMemento.IsMaximized = value;
        }
    }

    public void Initialize()
    {
        if (_initialized)
            throw new InvalidOperationException($"{nameof(IMainWindowMementoWrapper)} is already initialized");
        _initialized = true;
        var localApplicationDataPath=Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        const string company = "GazpromProject";
        const string applicationName = "AnchorCalc";
        const string settingsFolderName = "Settings";
        var settingsPath = Path.Combine(localApplicationDataPath, company, applicationName, settingsFolderName);
        _settingsFilePath = Path.Combine(settingsPath, "MainWindowMemento.json");
        Directory.CreateDirectory(settingsPath);
        if (!File.Exists(_settingsFilePath))
            return;
        var serializedMemento = File.ReadAllText(_settingsFilePath);
        _mainWindowMemento=JsonConvert.DeserializeObject<MainWindowMemento>(serializedMemento);
    }

    private void EnsureInitilized()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(IMainWindowMementoWrapper)} is not initialized");
    }
}