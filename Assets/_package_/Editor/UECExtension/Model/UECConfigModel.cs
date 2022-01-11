using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UEC.Event;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace UEC
{
    [Serializable]
    class ConfigItem
    {
        public string Username;
        public string Token;
        public List<string> Scopes = new List<string>();
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


//        private List<(string, string, List<string>)> _items;

        private List<ConfigItem> _items;

        public bool IsDirty { get; set; } = false;

        public UECConfigModel()
        {
            EventCenter.Register(this);
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


        /// <summary>
        /// todo 思考更好的方式将数据下发给View同时数据格式解耦
        /// </summary>
        /// <returns></returns>
        [EventTag]
        public List<(string, string, List<string>)> GetItems()
        {
            var list = new List<(string, string, List<string>)>(_items.Count);
            foreach (var item in _items)
            {
                list.Add((item.Username, item.Token, item.Scopes));
            }

            return list;
        }

        [EventTag]
        public int GetItemsCount()
        {
            return _items.Count;
        }

        [EventTag]
        private bool AddItem(string username, string token, List<string> scopes)
        {
            // todo check
            var item = _items.Where(i => i.Username == username).Select(i => i);

            // 已经存在同名username不能添加
            if (item.Any())
            {
                Debug.LogError($"已经存在{username}不能添加");
                return false;
            }

            var ci = new ConfigItem()
            {
                Username = username,
                Token = token,
                Scopes = scopes
            };
            
            _items.Add(ci);
            IsDirty = true;
            return true;
        }

        [EventTag]
        private bool RemoveItem(string username)
        {
            var item = _items.Where(i => i.Username == username).Select(i => i);
            if (!item.Any())
            {
                return false;
            }

            _items.Remove(item.First());
            IsDirty = true;
            return true;
        }

        [EventTag]
        private bool ModifyItem(string originalUsername, string username, string token, List<string> scopes)
        {
            // todo check username
            
            var item = _items.Where(i => i.Username == originalUsername).Select(i => i);

            // 不存在originalUsername的记录,则直接添加
            var configItems = item as ConfigItem[] ?? item.ToArray();
            if (!configItems.Any())
            {
                return AddItem(username, token, scopes);
            }
            
            var ci = configItems.First();
            ci.Username = username;
            ci.Token = token;
            ci.Scopes = scopes;

            IsDirty = true;
            return true;
        }


//        [EventTag]
//        public List<ConfigItem> GetItems()
//        {
//            return Items;
//        }
//
//        [EventTag]
//        public bool AddItem(ConfigItem item)
//        {
//            if (Items.Contains(item))
//            {
//                return false;
//            }
//
//            Items.Add(item);
//            IsDirty = true;
//            return true;
//        }
//
//        [EventTag]
//        public bool RemoveItemByIndex(int index)
//        {
//            IsDirty = true;
//            return Items.Count > index && RemoveItem(Items[index]);
//        }
//
//        [EventTag]
//        private bool RemoveItem(ConfigItem item)
//        {
//            if (!Items.Contains(item))
//            {
//                return false;
//            }
//
//            Items.Remove(item);
//            IsDirty = true;
//            return true;
//        }
//
//
//        public void SetUsername(ConfigItem item, string username)
//        {
//            if (item == null)
//            {
//                Debug.LogError("item is null");
//                return;
//            }
//
//            item.Username = username;
//        }

        #region file operate

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

        #endregion
    }
}