using CommunityToolkit.Mvvm.ComponentModel;
using TextCodec.Core;
using Windows.ApplicationModel.Resources;

namespace TextCodec.ViewModels
{
    internal sealed class HashViewModel : ObservableObject
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;
        private static ResourceLoader resource_loader = ResourceLoader.GetForViewIndependentUse();

        private string selectedHashTextPreprocessMode;
        private string selectedHashMode;

        public static string GetTranslation(string resource)
        {
            return resource_loader.GetString(resource);
        }

        public string SelectedHashTextPreprocessMode
        {
            get { return selectedHashTextPreprocessMode ??= AppSettings.HashTextPreprocessMode; }
            set
            {
                SetProperty(ref selectedHashTextPreprocessMode, value);
                AppSettings.HashTextPreprocessMode = value;
            }
        }

        public string SelectedHashMode
        {
            get { return selectedHashMode ??= AppSettings.HashMode; }
            set
            {
                SetProperty(ref selectedHashMode, value);
                AppSettings.HashMode = value;
            }
        }
    }
}
