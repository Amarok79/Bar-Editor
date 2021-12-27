// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using Microsoft.UI.Xaml.Controls;


namespace Bar;

public sealed partial class GinsPage : Page
{
    public GinsPageViewModel ViewModel => (GinsPageViewModel) DataContext;


    public GinsPage()
    {
        InitializeComponent();
        DataContext = App.Current.CreateViewModel<GinsPageViewModel>();
    }
}
