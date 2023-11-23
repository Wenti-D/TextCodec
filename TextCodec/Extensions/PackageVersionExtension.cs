using System;
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
