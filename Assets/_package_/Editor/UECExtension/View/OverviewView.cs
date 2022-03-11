using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class ItemContext
    {
        public VisualElement Element;
        public ConfigItem ConfigItem;
    }

    public class OverviewView : View<UECUI>
    {
        private VisualElementCache _cache;
        private VisualElementPool _pool;

        private VisualElement _noneTip;
        private VisualElement _itemListRoot;

        // private ItemContext _selectedItemContext;

        private Button _addBtn;
        private Button _removeBtn;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("list_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/overview_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            _pool.SetGetAction(element =>
            {
                element.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
            });
            _noneTip = _cache.Get("none_tip");
            _itemListRoot = _cache.Get("item_list_root");

            _addBtn = _cache.Get<Button>("add_item_btn");
            _addBtn.clicked += AddItem;

            _removeBtn = _cache.Get<Button>("remove_item_btn");
            _removeBtn.clicked += RemoveItem;
            _removeBtn.SetEnabled(false);
            Refresh();
        }


        public void Refresh()
        {
            var items = UI.UecConfig.Items;

            _noneTip.SetDisplay(items.Count <= 0);
            _itemListRoot.SetDisplay(items.Count > 0);

            ClearItemList();
            DrawItemList();
        }

        // public void SetUsername(string name)
        // {
        //     if (_selectedItemContext == null)
        //     {
        //         Debug.LogError("no select item");
        //         return;
        //     }
        //
        //     _selectedItemContext.ConfigItem.Username = name;
        // }

        // public void SetToken(string token)
        // {
        //     if (_selectedItemContext == null)
        //     {
        //         Debug.LogError("no select item");
        //         return;
        //     }
        //
        //     _selectedItemContext.ConfigItem.Token = token;
        // }

        // public void AddScope(string scope)
        // {
        //     if (_selectedItemContext == null)
        //     {
        //         Debug.LogError("no select item");
        //         return;
        //     }
        //
        //     // todo scope check
        //     _selectedItemContext.ConfigItem.AddScope(scope);
        // }

        // public void RemoveScope(string scope)
        // {
        //     if (_selectedItemContext == null)
        //     {
        //         Debug.LogError("no select item");
        //         return;
        //     }
        //
        //     _selectedItemContext.ConfigItem.RemoveScope(scope);
        // }
        //
        // public void ModifyScope(string previousValue, string newValue)
        // {
        //     if (_selectedItemContext == null)
        //     {
        //         Debug.LogError("no select item");
        //         return;
        //     }
        //
        //     _selectedItemContext.ConfigItem.ModifyScope(previousValue, newValue);
        // }

        private void AddItem()
        {
            var element = _pool.Get();
            var item = new ConfigItem()
            {
                Username = "*",
                Token = "*",
            };

            var context = new ItemContext {Element = element, ConfigItem = item};
            DrawItem(context);
            OnItemSelect(context);
        }

        private void RemoveItem()
        {
            if (_selectedItemContext == null)
            {
                Debug.LogError("Remove fail no select item");
                return;
            }

            var temp = _itemListRoot;
            var element = _selectedItemContext.Element;
            var config = _selectedItemContext.ConfigItem;
            var index = temp.IndexOf(element);
            var ev = temp.ElementAt(index);
            _pool.Return(ev);
            temp.RemoveAt(index);
            UI.UecConfig.RemoveItem(config);
            _selectedItemContext = null;
            _removeBtn.SetEnabled(false);
            UI.GetView<DetailView>().Hide();
        }

        private void DrawItemList()
        {
            var items = UI.UecConfig.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var element = _pool.Get();
                var item = items[i];

                var context = new ItemContext {Element = element, ConfigItem = item};
                DrawItem(context);
            }
        }

        private void DrawItem(ItemContext context)
        {
            var element = context.Element;
            var config = context.ConfigItem;
            _itemListRoot.Add(element);

            var uLab = element.Q<Label>("username");
            var tLab = element.Q<Label>("token");
            var sLab = element.Q<Label>("scopes");

            uLab.text = config.Username;
            tLab.text = config.Token;
            sLab.text = config.GetScopesOverview();

            element.Q<Button>().clickable = new Clickable(() =>
            {
                OnItemSelect(context);
                Debug.Log("click");
            });
        }

        private void OnItemSelect(ItemContext context)
        {
            if (_selectedItemContext != null)
            {
                OnItemUnSelect(_selectedItemContext);
            }

            _selectedItemContext = context;
            _removeBtn.SetEnabled(true);
            context.Element.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.25f));
            UI.GetView<DetailView>().Refresh(context);
        }

        private void OnItemUnSelect(ItemContext context)
        {
            context.Element.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
        }

        private void ClearItemList()
        {
            var temp = _itemListRoot;
            while (temp.childCount > 0)
            {
                var ev = temp.ElementAt(0);
                _pool.Return(ev);
                temp.RemoveAt(0);
            }
        }
    }
}