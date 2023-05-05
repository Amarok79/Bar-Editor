// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;


public sealed partial class GinsPage : Page
{
    public GinsPageVm ViewModel => (GinsPageVm)DataContext;


    public GinsPage()
    {
        InitializeComponent();

        DataContext = App.Current.CreateViewModel<GinsPageVm>();
    }
}
