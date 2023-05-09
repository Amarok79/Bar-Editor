// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;


public class GinsPageVm : ObservableObject
{
    private readonly IFlurlClient mClient;


    public IAsyncRelayCommand AddCommand { get; }

    public IAsyncRelayCommand RefreshCommand { get; }

    public IRelayCommand BeginEditCommand { get; }

    public IAsyncRelayCommand DeleteCommand { get; }

    public IAsyncRelayCommand PublishCommand { get; }

    public IAsyncRelayCommand UnPublishCommand { get; }

    public IAsyncRelayCommand SaveCommand { get; }

    public IRelayCommand CancelEditCommand { get; }


    public ObservableCollection<GinVm> Items { get; } = new();

    public AdvancedCollectionView ItemsView { get; set; }


    private GinVm? mSelectedItem;

    public GinVm? SelectedItem
    {
        get => mSelectedItem;
        set
        {
            SetProperty(ref mSelectedItem, value);

            EditedItem = value != null ? new GinVm(value) : null;

            InEditMode = false;
        }
    }


    private GinVm? mEditedItem;

    public GinVm? EditedItem
    {
        get => mEditedItem;
        set => SetProperty(ref mEditedItem, value);
    }


    private Boolean mInEditMode;

    public Boolean InEditMode
    {
        get => mInEditMode;
        set => SetProperty(ref mInEditMode, value);
    }


    public GinsPageVm(
        IConfiguration configuration
    )
    {
        var url = configuration.GetValue<String>("Backend:Url");
        var apiKey = configuration.GetValue<String>("Backend:ApiKey");
        mClient = new FlurlClient(url).WithHeader("Api-Key", apiKey);

        AddCommand = new AsyncRelayCommand(_AddAsync);
        RefreshCommand = new AsyncRelayCommand(_RefreshAsync);

        BeginEditCommand = new RelayCommand(_BeginEdit);
        CancelEditCommand = new RelayCommand(_CancelEdit);
        DeleteCommand = new AsyncRelayCommand(_DeleteAsync);
        PublishCommand = new AsyncRelayCommand(_PublishAsync);
        UnPublishCommand = new AsyncRelayCommand(_UnPublishAsync);
        SaveCommand = new AsyncRelayCommand(_SaveAsync);

        ItemsView = new AdvancedCollectionView(Items, true);
        ItemsView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
    }


    private async Task _RefreshAsync()
    {
        InEditMode = false;

        SelectedItem = null;
        EditedItem = null;


        var items = await mClient.Request("/api/gins")
            .SetQueryParam("includeDrafts", "true")
            .GetJsonAsync<GinVm[]>();

        Items.Clear();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    private async Task _AddAsync()
    {
        var item = new GinVm {
            Id = Guid.NewGuid(),
            Name = "New Gin",
            Teaser = String.Empty,
            Description = String.Empty,
            Images = new List<String>(),
            IsDraft = true,
        };

        await mClient.Request("/api/gins", item.Id)
            .PutJsonAsync(item);


        Items.Add(item);

        SelectedItem = item;
        EditedItem = item;
        InEditMode = true;
    }

    private void _BeginEdit()
    {
        if (SelectedItem == null)
        {
            return;
        }

        InEditMode = true;
    }

    private void _CancelEdit()
    {
        InEditMode = false;
    }

    private async Task _SaveAsync()
    {
        if (EditedItem == null)
        {
            return;
        }

        var item = EditedItem;

        await mClient.Request("/api/gins", item.Id)
            .PutJsonAsync(item);

        Items.Remove(item);
        Items.Add(item);

        InEditMode = false;
        SelectedItem = item;
    }

    private async Task _DeleteAsync()
    {
        if (SelectedItem == null)
        {
            return;
        }

        var dialog = new ContentDialog {
            Title = "Delete?",
            Content = $"Delete '{SelectedItem.Name}'?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = App.Current.MainWindow.Content.XamlRoot,
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return;
        }

        await mClient.Request("/api/gins", SelectedItem.Id)
            .DeleteAsync();

        Items.Remove(SelectedItem);

        SelectedItem = null;
        EditedItem = null;

        InEditMode = false;
    }

    private async Task _PublishAsync()
    {
        if (SelectedItem == null)
        {
            return;
        }

        var item = await mClient.Request("/api/gins", SelectedItem.Id)
            .GetJsonAsync<GinVm>();

        item.IsDraft = false;
        SelectedItem.IsDraft = false;

        await mClient.Request("/api/gins", item.Id)
            .PutJsonAsync(item);
    }

    private async Task _UnPublishAsync()
    {
        if (SelectedItem == null)
        {
            return;
        }

        var item = await mClient.Request("/api/gins", SelectedItem.Id)
            .GetJsonAsync<GinVm>();

        item.IsDraft = true;
        SelectedItem.IsDraft = true;

        await mClient.Request("/api/gins", item.Id)
            .PutJsonAsync(item);
    }
}
