// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Bar.Pages;


public sealed class GinVm : ObservableValidator, IEquatable<GinVm>
{
    public Guid Id { get; set; }


    private String mName = String.Empty;

    [Required]
    public String Name
    {
        get => mName;
        set => SetProperty(ref mName, value, true);
    }


    private String mTeaser = String.Empty;

    [Required]
    public String Teaser
    {
        get => mTeaser;
        set => SetProperty(ref mTeaser, value, true);
    }


    private String mDescription = String.Empty;

    [Required]
    public String Description
    {
        get => mDescription;
        set => SetProperty(ref mDescription, value, true);
    }


    public List<String> Images { get; set; } = new();


    public String ImagesAsText
    {
        get => String.Join(", ", Images);
        set =>
            Images = value.Split(',')
                .Select(x => x.Trim())
                .ToList();
    }


    private Boolean mIsDraft;

    public Boolean IsDraft
    {
        get => mIsDraft;
        set => SetProperty(ref mIsDraft, value);
    }


    public GinVm()
    {
    }

    public GinVm(
        GinVm value
    )
    {
        Id = value.Id;
        Name = value.Name;
        Teaser = value.Teaser;
        Description = value.Description;
        Images = value.Images.ToList();
        IsDraft = value.IsDraft;
    }


    public Boolean Equals(
        GinVm? other
    )
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    public override Boolean Equals(
        Object? obj
    )
    {
        return ReferenceEquals(this, obj) || ( obj is GinVm other && Equals(other) );
    }

    public override Int32 GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static Boolean operator ==(
        GinVm? left,
        GinVm? right
    )
    {
        return Equals(left, right);
    }

    public static Boolean operator !=(
        GinVm? left,
        GinVm? right
    )
    {
        return !Equals(left, right);
    }
}
