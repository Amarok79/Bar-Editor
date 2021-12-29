// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;

public sealed partial class GinsPage : Page
{
    public GinsPageVm ViewModel => (GinsPageVm) DataContext;


    public GinsPage()
    {
        InitializeComponent();

        DataContext = App.Current.CreateViewModel<GinsPageVm>();
    }
}
