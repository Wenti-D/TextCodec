using CommunityToolkit.Mvvm.ComponentModel;
using TextCodec.Core;
using Windows.ApplicationModel.Resources;

namespace TextCodec.ViewModels
{
    internal sealed class CodecViewModel : ObservableObject
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;
        private static ResourceLoader resource_loader = ResourceLoader.GetForViewIndependentUse();

        private bool isEncodeWithSpaceChecked;
        private string selectedBaseSeriesTextPreprocessMode;
        private string selectedBase58Style;
        private string selectedChineseTelegraphCodeStyle;

        public static string GetTranslation(string resource)
        {
            return resource_loader.GetString(resource);
        }

        /// <summary>
        /// UTF 系列「编码时带空格」是否勾选
        /// </summary>
        public bool IsEncodeWithSpaceChecked
        {
            get { return isEncodeWithSpaceChecked = AppSettings.IsUtfEncodeWithSpace; }
            set
            {
                SetProperty(ref isEncodeWithSpaceChecked, value);
                AppSettings.IsUtfEncodeWithSpace = value;
            }
        }

        /// <summary>
        /// 已选的 Base 系列文本预编码模式
        /// </summary>
        public string SelectedBaseSeriesTextPreprocessMode
        {
            get { return selectedBaseSeriesTextPreprocessMode ??= AppSettings.BaseSeriesTextPreprocessMode; }
            set
            {
                SetProperty(ref selectedBaseSeriesTextPreprocessMode, value);
                AppSettings.BaseSeriesTextPreprocessMode = value;
            }
        }

        /// <summary>
        /// 已选的 Base58 样式
        /// </summary>
        public string SelectedBase58Style
        {
            get { return selectedBase58Style ??= AppSettings.Base58Style; }
            set
            {
                SetProperty(ref selectedBase58Style, value);
                AppSettings.Base58Style = value;
            }
        }

        /// <summary>
        /// 中文电码编码样式
        /// </summary>
        public string SelectedChineseTelegraphCodeStyle
        {
            get { return selectedChineseTelegraphCodeStyle ??= AppSettings.ChineseTelegraphCodeStyle; }
            set
            {
                SetProperty(ref selectedChineseTelegraphCodeStyle, value);
                AppSettings.ChineseTelegraphCodeStyle = value;
            }
        }
    }
}
