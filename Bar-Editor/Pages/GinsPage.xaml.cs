// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;

public sealed partial class GinsPage : Page
{
    public GinsPageViewModel ViewModel => (GinsPageViewModel) DataContext;


    public GinsPage()
    {
        InitializeComponent();

        DataContext = App.Current.CreateViewModel<GinsPageViewModel>();
        Loaded += _GinsPage_Loaded;
    }

    private void _GinsPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.XamlRoot = XamlRoot;
    }
}
