using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Graphics;
using Windows.Storage;

namespace TextCodec.Core
{
    public sealed class AppSettings : ObservableObject
    {
        private BackdropTypes? backdropTypes;
        private bool? isMainWindowMaximum;
        private ulong? mainWindowRect;
        private bool? isUtfEncodeWithSpace;
        private string? baseSeriesTextPreprocessMode;
        private string? base58Style;
        private ApplicationDataContainer local_settings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 选中的背景材质
        /// </summary>
        public BackdropTypes BackdropType
        {
            get => backdropTypes.Value;
            set
            {
                SetProperty(ref backdropTypes, value);
                local_settings.Values["BackdropType"] = backdropTypes.ToString();
            }
        }

        /// <summary>
        /// 窗口是否最大化
        /// </summary>
        public bool? IsMainWindowMaximum
        {
            get => isMainWindowMaximum;
            set
            {
                SetProperty(ref isMainWindowMaximum, value);
                local_settings.Values["IsMainWindowMaximum"] = isMainWindowMaximum;
            }
        }

        /// <summary>
        /// 主窗口位置大小
        /// </summary>
        public ulong? MainWindowRect
        {
            get => mainWindowRect;
            set
            {
                SetProperty(ref mainWindowRect, value);
                local_settings.Values["MainWindowRect"] = mainWindowRect;
            }
        }

        /// <summary>
        /// UTF「编码时加空格」设置
        /// </summary>
        public bool IsUtfEncodeWithSpace
        {
            get => isUtfEncodeWithSpace.Value;
            set
            {
                SetProperty(ref isUtfEncodeWithSpace, value);
                local_settings.Values["IsUtfEncodeWithSpace"] = isUtfEncodeWithSpace;
            }
        }

        /// <summary>
        /// Base 系列文本预编码方式
        /// </summary>
        public string BaseSeriesTextPreprocessMode
        {
            get => baseSeriesTextPreprocessMode;
            set
            {
                SetProperty(ref baseSeriesTextPreprocessMode, value);
                local_settings.Values["BaseSeriesTextPreprocessMode"] = baseSeriesTextPreprocessMode;
            }
        }

        /// <summary>
        /// Base 58 编码类型
        /// </summary>
        public string Base58Style
        {
            get => base58Style;
            set
            {
                SetProperty(ref base58Style, value);
                local_settings.Values["Base58Style"] = base58Style;
            }
        }

        public AppSettings()
        {
            Init();
            GetSettings();
        }

        private void Init()
        {
            if (local_settings.Values["IsInitialized"] is null)
            {
                local_settings.Values["IsInitialized"] = true;
                local_settings.Values["BackdropType"] = "Mica";
                local_settings.Values["IsUtfEncodeWithSpace"] = true;
                local_settings.Values["BaseSeriesTextPreprocessMode"] = "CodecPageModeUtf16Le/Text";
                local_settings.Values["Base58Style"] = "CodecPageBase58StdCharList";
            }
        }

        private void GetSettings()
        {
            backdropTypes = Enum.Parse<BackdropTypes>(local_settings.Values["BackdropType"] as string);
            isMainWindowMaximum = (bool)local_settings.Values["IsMainWindowMaximum"];
            mainWindowRect = (ulong)local_settings.Values["MainWindowRect"];
            isUtfEncodeWithSpace = (bool)local_settings.Values["IsUtfEncodeWithSpace"];
            baseSeriesTextPreprocessMode = local_settings.Values["BaseSeriesTextPreprocessMode"] as string;
            base58Style = local_settings.Values["Base58Style"] as string;
        }
    }
}
