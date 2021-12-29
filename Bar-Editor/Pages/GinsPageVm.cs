// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;

public class GinsPageVm : ObservableObject
{
    private readonly IConfiguration mConfiguration;
    private readonly IFlurlClient mClient;


    public IAsyncRelayCommand AddCommand { get; }

    public IAsyncRelayCommand RefreshCommand { get; }

    public IRelayCommand EditCommand { get; }

    public IAsyncRelayCommand DeleteCommand { get; }

    public IAsyncRelayCommand PublishCommand { get; }

    public IAsyncRelayCommand UnPublishCommand { get; }

    public IAsyncRelayCommand SaveCommand { get; }


    public ObservableCollection<GinVm> Items { get; } = new();

    public AdvancedCollectionView ItemsView { get; set; }


    private GinVm? mSelectedItem;

    public GinVm? SelectedItem
    {
        get => mSelectedItem;
        set
        {
            SetProperty(ref mSelectedItem, value);

            if (value is not null)
            {
                EditedItem = new GinVm() {
                    Id      = value.Id,
                    Name    = value.Name,
                    Teaser  = value.Teaser,
                    Images  = value.Images.ToList(),
                    IsDraft = value.IsDraft,
                };
            }

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


    public GinsPageVm(IConfiguration configuration)
    {
        mConfiguration = configuration;

        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");
        mClient = new FlurlClient(url).WithHeader("Api-Key", apiKey);

        AddCommand     = new AsyncRelayCommand(_AddAsync);
        RefreshCommand = new AsyncRelayCommand(_RefreshAsync);

        EditCommand      = new RelayCommand(_Edit);
        DeleteCommand    = new AsyncRelayCommand<UIElement>(_DeleteAsync);
        PublishCommand   = new AsyncRelayCommand(_PublishAsync);
        UnPublishCommand = new AsyncRelayCommand(_UnPublishAsync);
        SaveCommand      = new AsyncRelayCommand(_SaveAsync);

        ItemsView = new AdvancedCollectionView(Items, true);
        ItemsView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
    }



    private async Task _RefreshAsync()
    {
        var items = await mClient.Request("/api/gins")
           .SetQueryParam("includeDrafts", "true")
           .GetJsonAsync<GinVm[]>();

        Items.Clear();

        foreach (var item in items)
            Items.Add(item);

        InEditMode   = false;
        SelectedItem = null;
    }

    private async Task _AddAsync()
    {
        var item = new GinVm {
            Id      = Guid.NewGuid(),
            Name    = "Unnamed",
            Teaser  = String.Empty,
            Images  = new List<String>(),
            IsDraft = true,
        };

        await mClient.Request("/api/gins", item.Id)
           .PutJsonAsync(item);

        Items.Add(item);

        InEditMode   = true;
        SelectedItem = item;
    }


    private void _Edit()
    {
        InEditMode = true;
    }


    private async Task _SaveAsync()
    {
        if (EditedItem is null)
            return;

        await mClient.Request("/api/gins", EditedItem.Id)
           .PutJsonAsync(EditedItem);

        Items.Remove(EditedItem);
        Items.Add(EditedItem);

        InEditMode = false;
    }

    private async Task _DeleteAsync(UIElement? element)
    {
        if (SelectedItem is null)
            return;

        var dialog = new ContentDialog {
            Title             = "Delete?",
            Content           = $"Delete '{SelectedItem.Name}'?",
            PrimaryButtonText = "Delete",
            CloseButtonText   = "Cancel",
            XamlRoot          = element?.XamlRoot,
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
            return;

        await mClient.Request("/api/gins", SelectedItem.Id)
           .DeleteAsync();

        Items.Remove(SelectedItem);
    }

    private async Task _PublishAsync()
    {
        if (SelectedItem is null)
            return;

        var gin = await mClient.Request("/api/gins", SelectedItem.Id)
           .GetJsonAsync<GinVm>();

        gin.IsDraft = false;

        await mClient.Request("/api/gins", gin.Id)
           .PutJsonAsync(gin);

        Items.Remove(gin);
        Items.Add(gin);

        SelectedItem = gin;
    }

    private async Task _UnPublishAsync()
    {
        if (SelectedItem is null)
            return;

        var gin = await mClient.Request("/api/gins", SelectedItem.Id)
           .GetJsonAsync<GinVm>();

        gin.IsDraft = true;

        await mClient.Request("/api/gins", gin.Id)
           .PutJsonAsync(gin);

        Items.Remove(gin);
        Items.Add(gin);

        SelectedItem = gin;
    }
}
