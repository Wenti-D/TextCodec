using Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace TextCodec.ViewModels
{
    internal sealed class CodecViewModel
    {
        private static ResourceLoader resource_loader = ResourceLoader.GetForViewIndependentUse();

        public static string GetTranslation(string resource)
        {
            return resource_loader.GetString(resource);
        }
    }
}
