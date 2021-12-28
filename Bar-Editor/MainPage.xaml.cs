// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using Bar.Pages;
using Microsoft.UI.Xaml.Controls;


namespace Bar;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = App.Current.CreateViewModel<MainPageViewModel>();
    }

    private void NavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var item = args.SelectedItem as NavigationViewItem;

        if (Equals(item?.Tag, "GINS"))
            Frame.Navigate(typeof(GinsPage));
        else
            Frame.Content = null;
    }
}
