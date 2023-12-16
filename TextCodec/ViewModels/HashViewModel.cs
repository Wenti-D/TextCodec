using CommunityToolkit.Mvvm.ComponentModel;
using TextCodec.Core;
using Windows.ApplicationModel.Resources;

namespace TextCodec.ViewModels
{
    public sealed class HashViewModel : ObservableObject
    {
        private readonly AppSettings _appSettings;
        private static ResourceLoader resource_loader = ResourceLoader.GetForViewIndependentUse();

        private string selectedHashTextPreprocessMode;
        private string selectedHashMode;

        public static string GetTranslation(string resource)
        {
            return resource_loader.GetString(resource);
        }

        public string SelectedHashTextPreprocessMode
        {
            get { return selectedHashTextPreprocessMode ??= _appSettings.HashTextPreprocessMode; }
            set
            {
                SetProperty(ref selectedHashTextPreprocessMode, value);
                _appSettings.HashTextPreprocessMode = value;
            }
        }

        public string SelectedHashMode
        {
            get { return selectedHashMode ??= _appSettings.HashMode; }
            set
            {
                SetProperty(ref selectedHashMode, value);
                _appSettings.HashMode = value;
            }
        }

        public HashViewModel(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
    }
}
