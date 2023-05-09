// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

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

    private void NavigationView_OnSelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args
    )
    {
        var item = args.SelectedItem as NavigationViewItem;

        if (Equals(item?.Tag, "GINS"))
        {
            MyFrame.Navigate(typeof(GinsPage));
        }
        else
        {
            MyFrame.Content = null;
        }
    }
}
