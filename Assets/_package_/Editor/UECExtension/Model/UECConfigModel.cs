using System;
using System.Collections.Generic;
using System.IO;
using UEC.Event;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace UEC
{
    [Serializable]
    public class ConfigItem
    {
        public string Username;
        public string Token;
        public List<string> Scopes = new List<string>();

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

        public void AddScope(string scope)
        {
            if (Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} was already exist");
                return;
            }

            Scopes.Add(scope);
        }

        public void RemoveScope(string scope)
        {
            if (!Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} is not exist");
                return;
            }

            Scopes.Remove(scope);
        }

        public void ModifyScope(string old, string scope)
        {
            if (!Scopes.Contains(old))
            {
                Debug.LogError($"{old} is not exist");
                return;
            }

            var index = Scopes.IndexOf(old);
            Scopes[index] = scope;
        }
    }

    public class UECConfigModel
    {
        #region Static

        private const string UECConfigFile = ".uecconfig";

        public static string UECConfigPath => Path.Combine(UserProfilePath(), UECConfigFile);

        private static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        #endregion


        private List<ConfigItem> _items;

        public List<ConfigItem> Items => _items;

        public bool IsDirty { get; set; } = false;

        public UECConfigModel()
        {
            EventCenter.Register(this);
            LoadConfig();
        }

        [EventTag]
        private void ModelTestMethod(int index, string msg)
        {
            Debug.Log($"{index}, {msg}");
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

        [EventTag]
        public List<ConfigItem> GetItems()
        {
            return Items;
        }

        [EventTag]
        public bool AddItem(ConfigItem item)
        {
            if (Items.Contains(item))
            {
                return false;
            }

            Items.Add(item);
            IsDirty = true;
            return true;
        }

        [EventTag]
        public bool RemoveItemByIndex(int index)
        {
            IsDirty = true;
            return Items.Count > index && RemoveItem(Items[index]);
        }

        [EventTag]
        private bool RemoveItem(ConfigItem item)
        {
            if (!Items.Contains(item))
            {
                return false;
            }

            Items.Remove(item);
            IsDirty = true;
            return true;
        }
        
        

        public void SetUsername(ConfigItem item, string username)
        {
            if (item == null)
            {
                Debug.LogError("item is null");
                return;
            }

            item.Username = username;
        }

        private void CreateConfig()
        {
            Write("");
        }

        [EventTag]
        public void Revert()
        {
            LoadConfig();
            IsDirty = false;
        }

        [EventTag]
        public void Apply()
        {
            SaveConfig();
            LoadConfig();
            IsDirty = false;
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