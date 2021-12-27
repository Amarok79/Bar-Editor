// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;


namespace Bar;

public class GinsPageViewModel : ObservableObject
{
    private readonly IConfiguration mConfiguration;


    public ObservableCollection<GinDto> Gins { get; } = new();

    private GinDto? mSelectedGin;

    public GinDto? SelectedGin
    {
        get => mSelectedGin;
        set => SetProperty(ref mSelectedGin, value);
    }

    public IAsyncRelayCommand LoadGinsCommand { get; }

    public IAsyncRelayCommand SaveCommand { get; }


    public GinsPageViewModel(IConfiguration configuration)
    {
        mConfiguration = configuration;

        LoadGinsCommand = new AsyncRelayCommand(_LoadGinsAsync);
        SaveCommand     = new AsyncRelayCommand(_SaveGinAsync);
    }


    private async Task _LoadGinsAsync()
    {
        var url    = mConfiguration.GetValue<String>("Backend:Url");
        var apiKey = mConfiguration.GetValue<String>("Backend:ApiKey");

        var gins = await url.AppendPathSegment("/api/gins")
           .WithHeader("Api-Key", apiKey)
           .GetJsonAsync<GinDto[]>();

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


    public sealed class GinDto : ObservableValidator
    {
        public Guid Id { get; set; }


        private String mName;

        [Required]
        public String Name
        {
            get => mName;
            set => SetProperty(ref mName, value, true);
        }

        private String mTeaser;

        [Required]
        public String Teaser
        {
            get => mTeaser;
            set => SetProperty(ref mTeaser, value, true);
        }


        public List<String> Images { get; set; }

        public String ImagesAsText
        {
            get => String.Join(", ", Images);
            set =>
                Images = value.Split(',')
                   .Select(x => x.Trim())
                   .ToList();
        }
    }
}
