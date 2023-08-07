using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core
{
    public enum BackdropTypes
    {
        [Description("没东西")]
        None,
        [Description("云母")]
        Mica,
        [Description("暗色云母")]
        MicaAlt,
        [Description("亚克力")]
        Acrylic
    }
}
