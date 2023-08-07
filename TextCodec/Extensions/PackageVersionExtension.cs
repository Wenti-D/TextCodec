using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace TextCodec.Extensions
{
    internal static class PackageVersionExtension
    {
        /// <summary>
        /// 将 PackageVersion 转换为 Version
        /// </summary>
        /// <param name="packageVersion">PackageVersion</param>
        /// <returns></returns>
        public static Version ToVersion(this PackageVersion packageVersion)
        {
            return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
    }
}
