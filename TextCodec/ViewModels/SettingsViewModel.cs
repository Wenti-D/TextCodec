using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using TextCodec.Core;
using TextCodec.Extensions;
using Windows.ApplicationModel;

namespace TextCodec.ViewModels
{
    internal sealed class SettingsViewModel : ObservableObject
    {
        #region fields
        private List<string> sysBackdrops;
        private string? selectedBackdrop;
        private Version version = Package.Current.Id.Version.ToVersion();
        #endregion

        #region properties
        public List<string> SysBackdrops { get { return sysBackdrops; } }
        public AppSettings AppSettings { get => MainWindow.AppSettings; }

        public string? SelectedBackdrop
        {
            get => selectedBackdrop ??= Enum.GetName(AppSettings.BackdropType);
            set
            {
                if (SetProperty(ref selectedBackdrop, value) && value != null)
                {
                    AppSettings.BackdropType = (BackdropTypes)Enum.Parse(typeof(BackdropTypes), value);
                }
            }
        }

        public Version Version { get { return version; } }
        #endregion

        #region methods
        public SettingsViewModel()
        {
            sysBackdrops = new List<string>();

            foreach (var backdrop_type in Enum.GetValues(typeof(BackdropTypes)))
            {
                sysBackdrops.Add(backdrop_type.ToString());
            }
        }
        #endregion
    }
}
