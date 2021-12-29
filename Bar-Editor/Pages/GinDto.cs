// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Bar.Pages;

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

    public Boolean IsDraft { get; set; }

    public Boolean IsNotDraft => !IsDraft;
}
