﻿using AnchorCalc.ViewModels.MainWindow;
using Autofac;

namespace AnchorCalc.ViewModels;

public class RegistrationModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().InstancePerDependency();
    }
}