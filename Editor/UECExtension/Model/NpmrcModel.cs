using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UEC
{
    public class NpmrcConfigItem
    {
        public string registry;
        public string token;
    }

    public class NpmrcConfig
    {
        public List<NpmrcConfigItem> Items = new List<NpmrcConfigItem>();
    }

    public class NpmrcModel
    {
        private const string NpmrcFile = ".npmrc";

        private const string TagContent = "#[uecconfig]";
        public static string NpmrcPath => Path.Combine(UserProfilePath(), NpmrcFile);

        public static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private UECContext Context;

        public NpmrcModel(UECContext context)
        {
            Context = context;
        }

        public void Update()
        {
            if (!File.Exists(NpmrcPath))
            {
                File.WriteAllText(NpmrcPath, "");
            }

            var lines = File.ReadLines(NpmrcPath);
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

            var config = GetNpmrcConfig();
            foreach (var item in config.Items)
            {
                configLines.Add(TagContent);
                configLines.Add(item.registry);
                configLines.Add(item.token);
                configLines.Add(TagContent);
            }

            foreach (var line in configLines)
            {
                Debug.Log(line);
            }

            File.WriteAllLines(NpmrcPath, configLines);
        }

        private NpmrcConfig GetNpmrcConfig()
        {
            var config = new NpmrcConfig();
            var items = Context.UECConfigModel.GetItems();

            foreach (var npmrcItem in from item in items
                where !string.IsNullOrEmpty(item.Token)
                select new NpmrcConfigItem
                {
                    registry = $"@{item.Username}:registry=https://npm.pkg.github.com",
                    token = $"//npm.pkg.github.com/:_authToken={item.Token}",
                })
            {
                config.Items.Add(npmrcItem);
            }

            return config;
        }
    }
}