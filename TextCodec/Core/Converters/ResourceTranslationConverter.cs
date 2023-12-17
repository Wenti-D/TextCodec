using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Data;
using System;
using TextCodec.ViewModels;

namespace TextCodec.Core.Converters;

public class ResourceTranslationConverter : IValueConverter
{
    private readonly IServiceProvider serviceProvider;
    private readonly CodecViewModel viewModel;

    public ResourceTranslationConverter()
    {
        serviceProvider = Ioc.Default;
        viewModel = serviceProvider.GetRequiredService<CodecViewModel>();
    }

    public object Convert(object value, Type target_type, object parameter, string language)
    {
        return value is not string value_str ? value : viewModel.GetTranslation(value_str);
    }

    public object ConvertBack(object value, Type target_type, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
