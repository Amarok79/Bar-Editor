// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace Bar.Pages;

public class GinsPageViewModel : ObservableObject
{
    private readonly IConfiguration mConfiguration;


    public XamlRoot XamlRoot { get; set; }

    public ObservableCollection<GinDto> Gins { get; } = new();

    public AdvancedCollectionView GinsCollection { get; set; }

    private GinDto? mSelectedGin;

    public GinDto? SelectedGin
    {
        get => mSelectedGin;
        set => SetProperty(ref mSelectedGin, value);
    }


    public IAsyncRelayCommand LoadGinsCommand { get; }
    
    public IAsyncRelayCommand RefreshCommand { get; }

    public IAsyncRelayCommand SaveCommand { get; }

    public IAsyncRelayCommand AddCommand { get; }

    public IAsyncRelayCommand EditCommand { get; }

    public IAsyncRelayCommand DeleteCommand { get; }

    public IAsyncRelayCommand PublishCommand { get; }

    public IAsyncRelayCommand UnPublishCommand { get; }


    public GinsPageViewModel(IConfiguration configuration)
    {
        mConfiguration = configuration;

        LoadGinsCommand  = new AsyncRelayCommand(_LoadGinsAsync);
        RefreshCommand  = new AsyncRelayCommand(_LoadGinsAsync);
        SaveCommand      = new AsyncRelayCommand(_SaveGinAsync);
        AddCommand       = new AsyncRelayCommand(_AddGinAsync);
        DeleteCommand    = new AsyncRelayCommand(_DeleteGinAsync);
        PublishCommand   = new AsyncRelayCommand(_PublishGinAsync);
        UnPublishCommand = new AsyncRelayCommand(_UnPublishGinAsync);

        GinsCollection = new AdvancedCollectionView(Gins, true);
        GinsCollection.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
    }


    private async Task _LoadGinsAsync()
    {
        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        var gins = await url.AppendPathSegment("/api/gins")
           .WithHeader("Api-Key", apiKey)
           .SetQueryParam("includeDrafts", "true")
           .GetJsonAsync<GinDto[]>();

        Gins.Clear();

        foreach (var x in gins)
            Gins.Add(x);
    }

    private async Task _SaveGinAsync()
    {
        if (SelectedGin is null)
            return;

        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        await url.AppendPathSegments("api", "gins", SelectedGin.Id)
           .WithHeader("Api-Key", apiKey)
           .PutJsonAsync(SelectedGin);
    }

    private async Task _AddGinAsync()
    {
        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        var gin = new GinDto {
            Id     = Guid.NewGuid(),
            Name   = "Unbenannt",
            Teaser = "",
            Images = new List<String>(),
            IsDraft = true,
        };

        await url.AppendPathSegments("api", "gins", gin.Id)
           .WithHeader("Api-Key", apiKey)
           .PutJsonAsync(gin);

        Gins.Add(gin);
        SelectedGin = gin;
    }

    private async Task _DeleteGinAsync()
    {
        if (SelectedGin is null)
            return;

        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");



        var dialog = new ContentDialog {
            Title             = "Delete?",
            Content           = $"Delete '{SelectedGin.Name}'?",
            PrimaryButtonText = "Delete",
            CloseButtonText   = "Cancel",
            XamlRoot          = XamlRoot,
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
            return;

        await url.AppendPathSegments("api", "gins", SelectedGin.Id)
           .WithHeader("Api-Key", apiKey)
           .DeleteAsync();

        Gins.Remove(SelectedGin);
    }

    private async Task _PublishGinAsync()
    {
        if (SelectedGin is null)
            return;

        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        var gin = await url.AppendPathSegments("api", "gins", SelectedGin.Id)
           .WithHeader("Api-Key", apiKey)
           .GetJsonAsync<GinDto>();

        gin.IsDraft = false;

        await url.AppendPathSegments("api", "gins", gin.Id)
           .WithHeader("Api-Key", apiKey)
           .PutJsonAsync(gin);
    }

    private async Task _UnPublishGinAsync()
    {
        if (SelectedGin is null)
            return;

        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        var gin = await url.AppendPathSegments("api", "gins", SelectedGin.Id)
           .WithHeader("Api-Key", apiKey)
           .GetJsonAsync<GinDto>();

        gin.IsDraft = true;

        await url.AppendPathSegments("api", "gins", gin.Id)
           .WithHeader("Api-Key", apiKey)
           .PutJsonAsync(gin);
    }
}
