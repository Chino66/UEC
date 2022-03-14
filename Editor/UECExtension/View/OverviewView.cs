using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UEC.Event;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class OverviewView : View<UECUI>
    {
        private const string Message =
            "You have unsaved changes which would be lost id you continue this operation. Do you want to continue and discard unsaved changes?";

        private VisualElementCache _cache;
        private VisualElementPool _pool;

        private VisualElement _noneTip;
        private VisualElement _itemListRoot;

        private UECContext context => UI.Context;

        private ItemDraftContext _currentSelectItemContext;

        private Dictionary<string, ItemDraftContext> itemDraftContexts => context.ItemDraftContexts;

        private ItemDraftContext currentSelectItemContext
        {
            get { return context.CurrentSelectItemContext; }
            set
            {
                _removeBtn.SetEnabled(value != null);
                context.SetItemDraftContext(value);

                if (value != null)
                {
                    if (_currentSelectItemContext != null)
                    {
                        UnselectStyle(_currentSelectItemContext);
                    }

                    context.SetCurrentItemConfigUsername(value.ConfigItem.Username);
                    _currentSelectItemContext = value;
                    _currentSelectItemContext.Element.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.25f));
                }
                else
                {
                    context.SetCurrentItemConfigUsername(null);
                }


                UI.GetView<DetailView>().Refresh();
            }
        }

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

            var addBtn = _cache.Get<Button>("add_item_btn");
            addBtn.clicked += AddItem;

            _removeBtn = _cache.Get<Button>("remove_item_btn");
            _removeBtn.clicked += RemoveItem;
            _removeBtn.SetEnabled(false);
            Refresh();
        }

        public void Refresh()
        {
            var count = context.UECConfigModel.GetItemsCount();
            _noneTip.SetDisplay(count <= 0);
            _itemListRoot.SetDisplay(count > 0);

            ClearItemList();
            DrawItemList();
        }

        private void AddItem()
        {
            if (itemDraftContexts.ContainsKey("*"))
            {
                UI.GetView<TipView>().Error("Already add new Item");
                return;
            }

            var element = _pool.Get();

            var context = new ItemDraftContext
            {
                Element = element,
                ConfigItem = new ConfigItem()
                {
                    Username = "*",
                    Token = "*",
                    Scopes = new List<string>()
                },
                IsNew = true,
            };

            itemDraftContexts.Add(context.ConfigItem.Username, context);
            DrawItem(context);
            context.DrawItem();
            OnItemSelect(context);
        }

        private void RemoveItem()
        {
            if (currentSelectItemContext == null)
            {
                Debug.LogError("Remove fail no select item");
                return;
            }

            var temp = _itemListRoot;
            var element = currentSelectItemContext.Element;
            var config = currentSelectItemContext.ConfigItem;
            var index = temp.IndexOf(element);
            var ev = temp.ElementAt(index);
            _pool.Return(ev);
            temp.RemoveAt(index);

            context.RemoveItem(config.Username);
            currentSelectItemContext = null;
            _removeBtn.SetEnabled(false);
            UI.GetView<DetailView>().Hide();
        }

        private void DrawItemList()
        {
            var items = context.UECConfigModel.GetItems();

            for (var i = 0; i < items.Count; i++)
            {
                var element = _pool.Get();
                var item = items[i];

                var context = new ItemDraftContext
                {
                    Element = element,
                    ConfigItem = item,
                    IsNew = false,
                    OriginalUsername = item.Username
                };
                DrawItem(context);
                context.DrawItem();

                itemDraftContexts.Add(context.ConfigItem.Username, context);
            }

            if (itemDraftContexts.TryGetValue(this.context.CurrentItemConfigUsername, out var draftContext))
            {
                // currentSelectItemContext = draftContext;
                OnItemSelect(draftContext);
            }
        }

        private void DrawItem(ItemDraftContext draftContext)
        {
            var element = draftContext.Element;
            _itemListRoot.Add(element);
            element.Q<Button>().clickable = new Clickable(() => { OnItemSelect(draftContext); });
        }

        private void OnItemSelect(ItemDraftContext draftContext)
        {
            currentSelectItemContext = draftContext;
        }

        private void UnselectStyle(ItemDraftContext draftContext)
        {
            draftContext.Element.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
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

            itemDraftContexts.Clear();
        }
    }
}