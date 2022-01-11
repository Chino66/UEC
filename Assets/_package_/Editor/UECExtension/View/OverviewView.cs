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
    /// <summary>
    /// todo 需要实现一个数据绑定,ConfigItemDraft和View元素的绑定,所有修改都通过View变化到ConfigItemDraft上,然后最终更新Model层
    /// todo list通过index作为key关联
    /// </summary>
    public class ConfigItemDraft
    {
        public string OriginalUsername;
        public string Username;
        public string Token;
        public List<string> Scopes = new List<string>();

        public ConfigItemDraft(string username, string token, List<string> scopes)
        {
            Instantiate(username, token, scopes);
        }

        public ConfigItemDraft((string, string, List<string>) item)
        {
            var (username, token, scopes) = item;
            Instantiate(username, token, scopes);
        }

        private void Instantiate(string username, string token, List<string> scopes)
        {
            OriginalUsername = username;
            Username = username;
            Token = token;
            Scopes = scopes;
        }

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

        public bool AddScope(string scope)
        {
            if (Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} was already exist");
                return false;
            }

            Scopes.Add(scope);
            return true;
        }

        public bool RemoveScope(string scope)
        {
            if (!Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} is not exist");
                return false;
            }

            Scopes.Remove(scope);
            return true;
        }

        public bool ModifyScope(string old, string scope)
        {
            if (!Scopes.Contains(old))
            {
                Debug.LogError($"{old} is not exist");
                return false;
            }

            var index = Scopes.IndexOf(old);
            Scopes[index] = scope;
            return true;
        }
    }

    public class ItemDraftContext
    {
        public VisualElement Element;
        public ConfigItemDraft ConfigItemDraft;
        public bool IsDirty;
        public bool IsNew;
    }

    public class OverviewView : View<UECUI>
    {
        private VisualElementCache _cache;
        private VisualElementPool _pool;

        private VisualElement _noneTip;
        private VisualElement _itemListRoot;

        private ItemDraftContext DraftContext => UI.DraftContext;

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
            var count = (int) EventCenter.SendEvent("UECConfigModel", "GetItemsCount");
            _noneTip.SetDisplay(count <= 0);
            _itemListRoot.SetDisplay(count > 0);

            ClearItemList();
            DrawItemList();
        }

//        public void SetUsername(string name)
//        {
//            if (_selectedItemDraftContext == null)
//            {
//                Debug.LogError("no select item");
//                return;
//            }
//
//            _selectedItemDraftContext.ConfigItemDraft.Username = name;
//        }

//        public void SetToken(string token)
//        {
//            if (_selectedItemDraftContext == null)
//            {
//                Debug.LogError("no select item");
//                return;
//            }
//
//            _selectedItemDraftContext.ConfigItemDraft.Token = token;
//        }

//        public void AddScope(string scope)
//        {
//            if (_selectedItemDraftContext == null)
//            {
//                Debug.LogError("no select item");
//                return;
//            }
//
//            // todo scope check
//            _selectedItemDraftContext.ConfigItemDraft.AddScope(scope);
//        }
//
//        public void RemoveScope(string scope)
//        {
//            if (_selectedItemDraftContext == null)
//            {
//                Debug.LogError("no select item");
//                return;
//            }
//
//            _selectedItemDraftContext.ConfigItemDraft.RemoveScope(scope);
//        }

        public void ModifyScope(string previousValue, string newValue)
        {
            if (DraftContext == null)
            {
                Debug.LogError("no select item");
                return;
            }

            DraftContext.ConfigItemDraft.ModifyScope(previousValue, newValue);
        }

        private void AddItem()
        {
            var element = _pool.Get();

            var context = new ItemDraftContext
            {
                Element = element,
                ConfigItemDraft = new ConfigItemDraft("*", "*", new List<string>()),
                IsNew = true
            };

            DrawItem(context);
            OnItemSelect(context);
        }

        private void RemoveItem()
        {
            if (DraftContext == null)
            {
                Debug.LogError("Remove fail no select item");
                return;
            }

            var temp = _itemListRoot;
            var element = DraftContext.Element;
            var config = DraftContext.ConfigItemDraft;
            var index = temp.IndexOf(element);
            var ev = temp.ElementAt(index);
            _pool.Return(ev);
            temp.RemoveAt(index);
            EventCenter.SendEvent("UECConfigModel", "RemoveItem", config.Username);
            UI.SetDraftContext(null);
            _removeBtn.SetEnabled(false);
            UI.GetView<DetailView>().Hide();
        }

        private void DrawItemList()
        {
            var items = (List<(string, string, List<string>)>) EventCenter.SendEvent("UECConfigModel", "GetItems");
            for (int i = 0; i < items.Count; i++)
            {
                var element = _pool.Get();
                var item = items[i];

                var context = new ItemDraftContext
                {
                    Element = element,
                    ConfigItemDraft = new ConfigItemDraft(item),
                    IsNew = false
                };
                DrawItem(context);
            }
        }

        private void DrawItem(ItemDraftContext draftContext)
        {
            var element = draftContext.Element;
            var config = draftContext.ConfigItemDraft;
            _itemListRoot.Add(element);

            var uLab = element.Q<Label>("username");
            var tLab = element.Q<Label>("token");
            var sLab = element.Q<Label>("scopes");

            uLab.text = config.Username;
            tLab.text = config.Token;
            sLab.text = config.GetScopesOverview();

            element.Q<Button>().clickable = new Clickable(() =>
            {
                OnItemSelect(draftContext);
                Debug.Log("click");
            });
        }

        private void OnItemSelect(ItemDraftContext draftContext)
        {
            if (DraftContext != null)
            {
                OnItemUnSelect(DraftContext);
            }

            UI.SetDraftContext(draftContext);
            _removeBtn.SetEnabled(true);
            draftContext.Element.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.25f));

            UI.GetView<DetailView>().Refresh(draftContext);
        }

        private const string Message =
            "You have unsaved changes which would be lost id you continue this operation. Do you want to continue and discard unsaved changes?";

        private void OnItemUnSelect(ItemDraftContext draftContext)
        {
            if (draftContext.IsDirty)
            {
//                if (EditorUtility.DisplayDialog("Discard unsaved changes ", Message, "Continue", "Cancel"))
//                {
//                    // todo unsaved changes
//                    Debug.Log("click Continue btn");
//                }
//                else
//                {
//                    // todo return editor
//                    Debug.Log("click Cancel btn");
//                }
            }

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
        }
    }
}