﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace AnchorCalc.Views.MainWindow.Controls
{
    /// <summary>
    /// Interaction logic for SurfacePlotControl.xaml
    /// </summary>
    public partial class SurfacePlotControl : UserControl
    {
        public SurfacePlotControl()
        {
            InitializeComponent();
            
            Viewport.ZoomExtentsGesture = new KeyGesture(Key.Space);

        }
    }
}