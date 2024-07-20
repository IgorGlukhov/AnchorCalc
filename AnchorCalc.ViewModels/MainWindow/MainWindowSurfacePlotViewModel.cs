using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using AnchorCalc.Domain.Factories;
using AnchorCalc.Infrastructure.Calc;
using AnchorCalc.ViewModels.Anchors;
using AnchorCalc.ViewModels.Commands;
using AnchorCalc.ViewModels.Plotters;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowSurfacePlotViewModel : ViewModel, IMainWindowSurfacePlotViewModel
{
    private readonly Command _addCoordinateContainersCommand;
    private readonly Command _enterPropertiesCommand;
    private readonly List<TextBox> _listTextBoxX = [];
    private readonly List<TextBox> _listTextBoxY = [];
    private double[,] _anchorCoordinatesX = { { -0.110, -0.110, 0.110, 0.110 } };
    private double[,] _anchorCoordinatesY = { { -0.110, 0.110, -0.110, 0.110 } };

    private int _anchorCount = 4;
    private StackPanel _anchorForceStack = new();
    private string _anchorValidateContact = string.Empty;
    private string _anchorValidateExcavation = string.Empty;
    private string _anchorValidateSplitting = string.Empty;
    private string _anchorValidateSteel = string.Empty;

    private double _basePlateLength = 300;
    private double _basePlateWidth = 300;
    private double _concreteBaseLength = 400;
    private double _concreteBaseWidth = 400;

    private double _concreteResistance = 18.5;
    private double _crackedNormativeForce;
    private double _criticEdgeDistance;
    private double _criticInterAxialDistance;

    private Point3D[,] _dataPoints = new Point3D[1, 1];

    private double _diameter;
    private double _factBaseHeight = 250;

    private double _force = 10;
    private double[,] _forceDataArray = new double[1, 1];
    private double _gammaNc;
    private double _gammaNp;
    private double _gammaNs;
    private double _gammaNsp;
    private bool _isCracked = true;
    private double _localDeformation = 0.0015;
    private double _minBaseHeight;


    private double _momentX = 3;

    private double _momentY = 10;
    private double _normativeResistance;
    private StackPanel _numberAnchorStack = new();
    private StackPanel _numberCoordinatesStack = new();
    private double _phiC;

    private double _sealingDepth;

    private string _title = "Напряжения в бетоне основания";

    private int _triangulation = 100;
    private double _uncrackedNormativeForce = 25;

    private string _xAxisLabel = "X";
    private StackPanel _xCoordinatesStack = new();
    private double[,] _xData2DArray = new double[1, 1];

    private string _yAxisLabel = "Y";
    private StackPanel _yCoordinatesStack = new();
    private double[,] _yData2DArray = new double[1, 1];

    private string _zAxisLabel = "Z";
    private double[,] _zData2DArray = new double[1, 1];
    private readonly IAnchorCollectionViewModel _anchorCollectionViewModel;

    public MainWindowSurfacePlotViewModel(IFactory<IAnchorCollectionViewModel> anchorCollectionViewModelFactory)
    {
        _enterPropertiesCommand = new Command(EnterProperties);
        _addCoordinateContainersCommand = new Command(AddAnchorCoordinateContainers);
        _anchorCollectionViewModel = anchorCollectionViewModelFactory.Create();
        if (_anchorCollectionViewModel.Items != null)
        {
            _crackedNormativeForce = _anchorCollectionViewModel.Items[0].CrackedNormativeForce;
            _criticEdgeDistance = _anchorCollectionViewModel.Items[0].CriticEdgeDistance;
            _diameter = _anchorCollectionViewModel.Items[0].Diameter;
            _gammaNc = _anchorCollectionViewModel.Items[0].GammaNc;
            _gammaNp = _anchorCollectionViewModel.Items[0].GammaNp;
            _gammaNs = _anchorCollectionViewModel.Items[0].GammaNs;
            _gammaNsp = _anchorCollectionViewModel.Items[0].GammaNsp;
            _minBaseHeight = _anchorCollectionViewModel.Items[0].MinBaseHeight;
            _normativeResistance = _anchorCollectionViewModel.Items[0].NormativeResistance;
            _phiC = _anchorCollectionViewModel.Items[0].PhiC;
            _sealingDepth = _anchorCollectionViewModel.Items[0].SealingDepth;
            _criticInterAxialDistance = _anchorCollectionViewModel.Items[0].CriticInterAxialDistance;
        }

        ModelChange();
    }
    public StackPanel NumberCoordinatesStack
    {
        get => _numberCoordinatesStack;
        private set
        {
            _numberCoordinatesStack = value;
            OnPropertyChanged();
        }
    }

    public StackPanel NumberAnchorStack
    {
        get => _numberAnchorStack;
        private set
        {
            _numberAnchorStack = value;
            OnPropertyChanged();
        }
    }

    public StackPanel AnchorForceStack
    {
        get => _anchorForceStack;
        private set
        {
            _anchorForceStack = value;
            OnPropertyChanged();
        }
    }

    public StackPanel XCoordinatesStack
    {
        get => _xCoordinatesStack;
        private set
        {
            _xCoordinatesStack = value;
            OnPropertyChanged();
        }
    }

    public StackPanel YCoordinatesStack
    {
        get => _yCoordinatesStack;
        private set
        {
            _yCoordinatesStack = value;
            OnPropertyChanged();
        }
    }

    public ICommand EnterPropertiesCommand => _enterPropertiesCommand;
    public ICommand AddCoordinateContainersCommand => _addCoordinateContainersCommand;


    public double[,] ForceDataArray
    {
        get => _forceDataArray;
        private set
        {
            _forceDataArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] AnchorCoordinatesX
    {
        get => _anchorCoordinatesX;
        set
        {
            _anchorCoordinatesX = value;
            OnPropertyChanged();
        }
    }

    public double[,] AnchorCoordinatesY
    {
        get => _anchorCoordinatesY;
        set
        {
            _anchorCoordinatesY = value;
            OnPropertyChanged();
        }
    }

    public double[,] XData2DArray
    {
        get => _xData2DArray;
        private set
        {
            _xData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] YData2DArray
    {
        get => _yData2DArray;
        private set
        {
            _yData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] ZData2DArray
    {
        get => _zData2DArray;
        private set
        {
            _zData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double LocalDeformation
    {
        get => _localDeformation;
        set
        {
            _localDeformation = value;
            OnPropertyChanged();
        }
    }

    public double NormativeResistance
    {
        get => _normativeResistance;
        set
        {
            _normativeResistance = value;
            OnPropertyChanged();
        }
    }

    public double CrackedNormativeForce
    {
        get => _crackedNormativeForce;
        set
        {
            _crackedNormativeForce = value;
            OnPropertyChanged();
        }
    }

    public double UncrackedNormativeForce
    {
        get => _uncrackedNormativeForce;
        set
        {
            _uncrackedNormativeForce = value;
            OnPropertyChanged();
        }
    }

    public double ConcreteBaseWidth
    {
        get => _concreteBaseWidth;
        set
        {
            _concreteBaseWidth = value;
            OnPropertyChanged();
        }
    }

    public double ConcreteBaseLength
    {
        get => _concreteBaseLength;
        set
        {
            _concreteBaseLength = value;
            OnPropertyChanged();
        }
    }

    public double CriticInterAxialDistance
    {
        get => _criticInterAxialDistance;
        set
        {
            _criticInterAxialDistance = value;
            OnPropertyChanged();
        }
    }

    public double CriticEdgeDistance
    {
        get => _criticEdgeDistance;
        set
        {
            _criticEdgeDistance = value;
            OnPropertyChanged();
        }
    }

    public double MinBaseHeight
    {
        get => _minBaseHeight;
        set
        {
            _minBaseHeight = value;
            OnPropertyChanged();
        }
    }

    public double FactBaseHeight
    {
        get => _factBaseHeight;
        set
        {
            _factBaseHeight = value;
            OnPropertyChanged();
        }
    }

    public double PhiC
    {
        get => _phiC;
        set
        {
            _phiC = value;
            OnPropertyChanged();
        }
    }

    public double GammaNs
    {
        get => _gammaNs;
        set
        {
            _gammaNs = value;
            OnPropertyChanged();
        }
    }

    public double GammaNp
    {
        get => _gammaNp;
        set
        {
            _gammaNp = value;
            OnPropertyChanged();
        }
    }

    public double GammaNc
    {
        get => _gammaNc;
        set
        {
            _gammaNc = value;
            OnPropertyChanged();
        }
    }

    public double GammaNsp
    {
        get => _gammaNsp;
        set
        {
            _gammaNsp = value;
            OnPropertyChanged();
        }
    }

    public bool IsCracked
    {
        get => _isCracked;
        set
        {
            _isCracked = value;
            OnPropertyChanged();
        }
    }

    public string AnchorValidateSplitting
    {
        get => _anchorValidateSplitting;
        set
        {
            _anchorValidateSplitting = value;
            OnPropertyChanged();
        }
    }

    public string AnchorValidateExcavation
    {
        get => _anchorValidateExcavation;
        set
        {
            _anchorValidateExcavation = value;
            OnPropertyChanged();
        }
    }

    public string AnchorValidateContact
    {
        get => _anchorValidateContact;
        set
        {
            _anchorValidateContact = value;
            OnPropertyChanged();
        }
    }

    public string AnchorValidateSteel
    {
        get => _anchorValidateSteel;
        set
        {
            _anchorValidateSteel = value;
            OnPropertyChanged();
        }
    }

    public Point3D[,] DataPoints
    {
        get => _dataPoints;
        private set
        {
            _dataPoints = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string XAxisLabel
    {
        get => _xAxisLabel;
        set
        {
            _xAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public string YAxisLabel
    {
        get => _yAxisLabel;
        set
        {
            _yAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public string ZAxisLabel
    {
        get => _zAxisLabel;
        set
        {
            _zAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public bool ShowMiniCoordinates { get; } = true;

    public double Force
    {
        get => _force;
        set
        {
            _force = value;
            OnPropertyChanged();
        }
    }

    public double MomentX
    {
        get => _momentX;
        set
        {
            _momentX = value;
            OnPropertyChanged();
        }
    }

    public double MomentY
    {
        get => _momentY;
        set
        {
            _momentY = value;
            OnPropertyChanged();
        }
    }


    public double SealingDepth
    {
        get => _sealingDepth;
        set
        {
            _sealingDepth = value;
            OnPropertyChanged();
        }
    }

    public double Diameter
    {
        get => _diameter;
        set
        {
            _diameter = value;
            OnPropertyChanged();
        }
    }

    public double ConcreteResistance
    {
        get => _concreteResistance;
        set
        {
            _concreteResistance = value;
            OnPropertyChanged();
        }
    }

    public double BasePlateWidth
    {
        get => _basePlateWidth;
        set
        {
            _basePlateWidth = value;
            OnPropertyChanged();
        }
    }

    public double BasePlateLength
    {
        get => _basePlateLength;
        set
        {
            _basePlateLength = value;
            OnPropertyChanged();
        }
    }

    public int Triangulation
    {
        get => _triangulation;
        set
        {
            _triangulation = value;
            OnPropertyChanged();
        }
    }


    public int AnchorCount
    {
        get => _anchorCount;
        set
        {
            _anchorCount = value;
            OnPropertyChanged();
        }
    }

    private void AddAnchorCoordinateContainers()
    {
        NumberCoordinatesStack = new StackPanel();
        XCoordinatesStack = new StackPanel();
        YCoordinatesStack = new StackPanel();
        NumberAnchorStack = new StackPanel();
        AnchorForceStack = new StackPanel();
        _listTextBoxX.Clear();
        _listTextBoxY.Clear();

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxX = new TextBox
            {
                Height = 16,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxX;
            if (AnchorCoordinatesX.GetLength(1) > i)
                textBoxX.Text = (AnchorCoordinatesX[0, i] * 1000).ToString(CultureInfo.CurrentCulture);
            _listTextBoxX.Add(textBoxX);
            XCoordinatesStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxY = new TextBox
            {
                Height = 16,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxY;
            if (AnchorCoordinatesY.GetLength(1) > i)
                textBoxY.Text = (AnchorCoordinatesY[0, i] * 1000).ToString(CultureInfo.CurrentCulture);
            _listTextBoxY.Add(textBoxY);
            YCoordinatesStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxNumber = new TextBlock
            {
                Text = (i + 1).ToString(),
                Height = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxNumber;
            NumberCoordinatesStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBlock = new TextBlock
            {
                Height = 16,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBlock;
            if (ForceDataArray.GetLength(1) > i) textBlock.Text = $"{ForceDataArray[0, i]:0.00}";
            AnchorForceStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxNumber = new TextBlock
            {
                Text = (i + 1).ToString(),
                Height = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxNumber;
            NumberAnchorStack.Children.Add(border);
        }
    }

    private void EnterProperties()
    {
        if (_listTextBoxX.Count > 0 && _listTextBoxY.Count > 0)
        {
            AnchorCoordinatesX = new double[1, AnchorCount];
            AnchorCoordinatesY = new double[1, AnchorCount];
            for (var i = 0; i < AnchorCount; i++)
            {
                AnchorCoordinatesX[0, i] = double.Parse(_listTextBoxX[i].Text) / 1000;
                AnchorCoordinatesY[0, i] = double.Parse(_listTextBoxY[i].Text) / 1000;
            }
        }
        else
        {
            throw new Exception();
        }

        ModelChange();
    }

    private void ModelChange()
    {
        var calc = new Calc(Force, MomentX, MomentY, SealingDepth, Diameter, ConcreteResistance,
            LocalDeformation, BasePlateWidth, BasePlateLength, Triangulation,
            AnchorCoordinatesX, AnchorCoordinatesY, NormativeResistance, CrackedNormativeForce, UncrackedNormativeForce,
            ConcreteBaseWidth, ConcreteBaseLength, CriticInterAxialDistance, CriticEdgeDistance, MinBaseHeight,
            FactBaseHeight, PhiC, GammaNs, GammaNp, GammaNc, GammaNsp, IsCracked);
        ZData2DArray = calc.ConcreteTensionValues;
        XData2DArray = calc.BasePlateCoordinatesX;
        YData2DArray = calc.BasePlateCoordinatesY;
        AnchorValidateSteel = $"{calc.AnchorValidateSteel:0.00}%";
        AnchorValidateContact = $"{calc.AnchorValidateContact:0.00}%";
        AnchorValidateExcavation = $"{calc.AnchorValidateExcavation:0.00}%";
        AnchorValidateSplitting = $"{calc.AnchorValidateSplitting:0.00}%";
        ForceDataArray = calc.AnchorForce;
        DataPoints = PlotFromArray.Plot(XData2DArray, YData2DArray, ZData2DArray);
        AddAnchorCoordinateContainers();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}