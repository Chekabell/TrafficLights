﻿namespace traffic_lights;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell())
        {
            MaximumWidth = 400,
            MaximumHeight = 600,
        };
    }
}