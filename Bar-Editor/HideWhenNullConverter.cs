// Copyright (c) 2021, Olaf Kober <olaf.kober@outlook.com>

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;


namespace Bar;

public sealed class HideWhenNullConverter : IValueConverter
{
    public Object? Convert(Object? value, Type targetType, Object parameter, String language)
    {
        return ReferenceEquals(value, null) ? Visibility.Collapsed : Visibility.Visible;
    }

    public Object? ConvertBack(Object? value, Type targetType, Object parameter, String language)
    {
        throw new NotImplementedException();
    }
}
