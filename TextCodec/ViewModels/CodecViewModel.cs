using Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TextCodec.Core;
using System.Collections.ObjectModel;

namespace TextCodec.ViewModels
{
    internal sealed class CodecViewModel : ObservableObject
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;
        private static ResourceLoader resource_loader = ResourceLoader.GetForViewIndependentUse();

        private bool isEncodeWithSpaceChecked;
        private string selectedBaseSeriesTextPreprocessMode;
        private string selectedBase58Style;

        public static string GetTranslation(string resource)
        {
            return resource_loader.GetString(resource);
        }

        public bool IsEncodeWithSpaceChecked
        {
            get { return isEncodeWithSpaceChecked = AppSettings.IsUtfEncodeWithSpace; }
            set
            {
                SetProperty(ref isEncodeWithSpaceChecked, value);
                AppSettings.IsUtfEncodeWithSpace = value;
            }
        }

        public string SelectedBaseSeriesTextPreprocessMode
        {
            get { return selectedBaseSeriesTextPreprocessMode ??= AppSettings.BaseSeriesTextPreprocessMode; }
            set
            {
                SetProperty(ref selectedBaseSeriesTextPreprocessMode, value);
                AppSettings.BaseSeriesTextPreprocessMode = value;
            }
        }

        public string SelectedBase58Style
        {
            get { return selectedBase58Style ??= AppSettings.Base58Style; }
            set
            {
                SetProperty(ref selectedBase58Style, value);
                AppSettings.Base58Style = value;
            }
        }
    }
}
