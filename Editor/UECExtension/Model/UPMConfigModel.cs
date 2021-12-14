using System;
using System.IO;

namespace UEC
{
    public class UPMConfigModel
    {
        private const string UPMConfigFile = ".upmconfig.toml";

        public static string UPMConfigPath => Path.Combine(UserProfilePath(), UPMConfigFile);

        public static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static string SpecialFolder(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }
    }
}