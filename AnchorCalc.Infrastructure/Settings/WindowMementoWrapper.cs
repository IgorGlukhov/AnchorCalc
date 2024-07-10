using AnchorCalc.Domain.Settings;
using AnchorCalc.Infrastructure.Common;
using Newtonsoft.Json;

namespace AnchorCalc.Infrastructure.Settings;

internal abstract class WindowMementoWrapper<TMemento>(IPathService pathService) : IWindowMementoWrapperInitializer,
    IMainWindowMementoWrapper,
    IDisposable
    where TMemento : WindowMemento, new()
{
    private bool _initialized;
    private string _settingsFilePath = string.Empty;
    private TMemento _windowMemento = new();

    protected abstract string MementoName { get; }

    public double Left
    {
        get
        {
            EnsureInitialized();
            return _windowMemento.Left;
        }
        set
        {
            EnsureInitialized();
            _windowMemento.Left = value;
        }
    }

    public double Top
    {
        get
        {
            EnsureInitialized();
            return _windowMemento.Top;
        }
        set
        {
            EnsureInitialized();
            _windowMemento.Top = value;
        }
    }

    public double Width
    {
        get
        {
            EnsureInitialized();
            return _windowMemento.Width;
        }
        set
        {
            EnsureInitialized();
            _windowMemento.Width = value;
        }
    }

    public double Height
    {
        get
        {
            EnsureInitialized();
            return _windowMemento.Height;
        }
        set
        {
            EnsureInitialized();
            _windowMemento.Height = value;
        }
    }

    public bool IsMaximized
    {
        get
        {
            EnsureInitialized();
            return _windowMemento.IsMaximized;
        }
        set
        {
            EnsureInitialized();
            _windowMemento.IsMaximized = value;
        }
    }

    public void Initialize()
    {
        if (_initialized)
            throw new InvalidOperationException($"Wrapper for {nameof(TMemento)} is already initialized");
        _initialized = true;
        const string settingsFolderName = "Settings";
        var settingsPath = Path.Combine(pathService.ApplicationFolder, settingsFolderName);
        _settingsFilePath = Path.Combine(settingsPath, $"{MementoName}.json");
        Directory.CreateDirectory(settingsPath);
        if (!File.Exists(_settingsFilePath))
            return;
        var serializedMemento = File.ReadAllText(_settingsFilePath);
        _windowMemento = JsonConvert.DeserializeObject<TMemento>(serializedMemento)
                         ?? throw new InvalidOperationException("Deserialized memento can't be null");
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException($"Wrapper for {nameof(TMemento)} is not initialized");
    }

    public void Dispose()
    {
        EnsureInitialized();
        var serializedMemento = JsonConvert.SerializeObject(_windowMemento);
        File.WriteAllText(_settingsFilePath, serializedMemento);
    }
}