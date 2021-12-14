using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

namespace UEC
{
    public class ConfigItem
    {
        public string Username;
        public string Token;
        public List<string> Scopes;

        public string GetScopesOverview()
        {
            var context = "*";
            if (Scopes == null)
            {
                return context;
            }

            context = "";
            for (int i = 0; i < Scopes.Count; i++)
            {
                context += Scopes[i];
                if (i < Scopes.Count - 1)
                {
                    context += "|";
                }
            }

            return context;
        }
    }

    public class UECConfigModel
    {
        private const string UECConfigFile = ".uecconfig";

        public static string UECConfigPath => Path.Combine(UserProfilePath(), UECConfigFile);

        private static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private List<ConfigItem> _items;

        public List<ConfigItem> Items => _items;

        public UECConfigModel()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var file = new FileInfo(UECConfigPath);

            if (file.Exists == false)
            {
                CreateConfig();
            }

            var json = "";
            using (var reader = new StreamReader(UECConfigPath, System.Text.Encoding.Default))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            _items = JsonConvert.DeserializeObject<List<ConfigItem>>(json) ?? new List<ConfigItem>();
        }

        private void CreateConfig()
        {
            Write("");
        }

        private void SaveConfig()
        {
            var setting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(_items, Formatting.Indented, setting);
            Write(json);
        }

        private void Write(string content)
        {
            using (var sw = new StreamWriter(UECConfigPath, false, System.Text.Encoding.Default))
            {
                sw.Write(content);
                sw.Close();
            }
        }
    }
}