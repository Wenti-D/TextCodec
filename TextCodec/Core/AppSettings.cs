using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace TextCodec.Core
{
    public sealed class AppSettings : ObservableObject
    {
        private BackdropTypes? backdropTypes;
        private ApplicationDataContainer local_settings = ApplicationData.Current.LocalSettings;

        public BackdropTypes BackdropType
        {
            get
            {
                if (backdropTypes is not null)
                {
                    return backdropTypes.Value;
                }
                return Enum.Parse<BackdropTypes>("Mica");
            }
            set
            {
                SetProperty(ref backdropTypes, value);
                local_settings.Values["BackdropType"] = backdropTypes.ToString();
            }
        }

        public AppSettings()
        {
            if (local_settings.Values["BackdropType"] is null) { local_settings.Values["BackdropType"] = "Mica"; }
            backdropTypes = Enum.Parse<BackdropTypes>(local_settings.Values["BackdropType"] as string);

        }
    }
}
