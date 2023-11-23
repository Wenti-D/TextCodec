using Microsoft.UI.Xaml.Data;
using System;
using TextCodec.ViewModels;

namespace TextCodec.Core.Converters
{
    public class ResourceTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type target_type, object parameter, string language)
        {
            var value_str = value as string;
            return value_str == null ? value : CodecViewModel.GetTranslation(value_str);
        }

        public object ConvertBack(object value, Type target_type, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
