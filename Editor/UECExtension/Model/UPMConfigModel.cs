using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UEC
{
    public class UPMConfigItem
    {
        public string npmAuth;
        public string token;
        public string email;
        public string alwaysAuth;
    }

    public class UPMConfig
    {
        public List<UPMConfigItem> Items = new List<UPMConfigItem>();
    }

    /*
     * https://docs.unity3d.com/cn/current/Manual/upm-config-scoped.html
     */
    public class UPMConfigModel
    {
        private const string UPMConfigFile = ".upmconfig.toml";

        private const string TagContent = "#[uecconfig]";
        public static string UPMConfigPath => Path.Combine(UserProfilePath(), UPMConfigFile);

        public static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private UECContext Context;

        public UPMConfigModel(UECContext context)
        {
            Context = context;
        }

        public void Update()
        {
            var lines = File.ReadLines(UPMConfigPath);
            var configLines = new List<string>();

            var startRead = true;
            foreach (var line in lines)
            {
                if (line.Contains(TagContent))
                {
                    startRead = !startRead;

                    continue;
                }

                if (!startRead)
                {
                    continue;
                }

                configLines.Add(line);
            }

            var config = GetUPMConfig();
            foreach (var item in config.Items)
            {
                configLines.Add(TagContent);
                configLines.Add(item.npmAuth);
                configLines.Add(item.token);
                if (string.IsNullOrEmpty(item.email) == false)
                {
                    configLines.Add(item.email);
                }

                configLines.Add(item.alwaysAuth);
                configLines.Add(TagContent);
            }

            foreach (var line in configLines)
            {
                Debug.Log(line);
            }

            File.WriteAllLines(UPMConfigPath, configLines);
        }

        private UPMConfig GetUPMConfig()
        {
            var config = new UPMConfig();
            var items = Context.UECConfigModel.GetItems();
            foreach (var upmItem in from item in items
                where !string.IsNullOrEmpty(item.Token)
                select new UPMConfigItem
                {
                    npmAuth = $"[npmAuth.\"https://npm.pkg.github.com/@{item.Username}\"]",
                    token = $"token = \"{item.Token}\"",
                    email = "",
                    alwaysAuth = string.IsNullOrEmpty(item.Token) ? "alwaysAuth = false" : "alwaysAuth = true"
                })
            {
                config.Items.Add(upmItem);
            }

            return config;
        }

        public static string SpecialFolder(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }
    }
}