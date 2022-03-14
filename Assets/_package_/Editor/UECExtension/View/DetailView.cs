using System.IO;
using UEC.Event;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class ScopeContext
    {
        public VisualElement Element;
        public string Scope;
    }

    public class DetailView : View<UECUI>
    {
        private VisualElementCache _cache;
        private VisualElementPool _pool;

        private TextField _nameTf;
        private TextField _tokenTf;

        private VisualElement _scopeListRoot;
        private VisualElement _noneTip;

        private ScopeContext _scopeContext;

        private ScopeContext ScopeContext
        {
            get { return _scopeContext; }

            set
            {
                _removeBtn.SetEnabled(value != null);

                _scopeContext = value;
            }
        }

        private Button _addBtn;
        private Button _removeBtn;

        private UECContext context => UI.Context;
        private ItemDraftContext itemContext => context.ItemContext;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("detail_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/detail_scope_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);

            _nameTf = _cache.Get<TextField>("name_tf");
            _nameTf.RegisterValueChangedCallback(evt =>
            {
                if (itemContext.ConfigItem.Username == evt.newValue)
                {
                    return;
                }

                itemContext.SetUsername(evt.newValue);
                context.IsDirty = true;
                
            });
            _tokenTf = _cache.Get<TextField>("token_tf");
            _tokenTf.RegisterValueChangedCallback(evt =>
            {
                if (itemContext.ConfigItem.Token == evt.newValue)
                {
                    return;
                }

                itemContext.SetToken(evt.newValue);
                context.IsDirty = true;
            });

            _scopeListRoot = _cache.Get("scope_item_list_root");
            _noneTip = _cache.Get("none_tip");

            _addBtn = _cache.Get<Button>("add_item_btn");
            _addBtn.clicked += AddScope;

            _removeBtn = _cache.Get<Button>("remove_item_btn");
            _removeBtn.clicked += RemoveSelectScope;

            ScopeContext = null;
            Hide();
        }


        public void Refresh()
        {
            if (itemContext == null)
            {
                Hide();
                return;
            }

            ShowItemDetail(itemContext);
        }

        private void ShowItemDetail(ItemDraftContext draftContext)
        {
            var config = draftContext.ConfigItem;
            _nameTf.value = config.Username;
            _tokenTf.value = config.Token;

            ClearScopeList();
            DrawScopeList(draftContext);

            Show();
        }


        private void AddScope()
        {
            var scopeContext = new ScopeContext {Element = _pool.Get(), Scope = "*"};
            ScopeContext = scopeContext;
            var rst = itemContext.AddScope(scopeContext.Scope);
            if (rst)
            {
                DrawScope(scopeContext);
                context.IsDirty = true;
            }
        }

        private void RemoveSelectScope()
        {
            if (ScopeContext == null)
            {
                return;
            }

            var index = _scopeListRoot.IndexOf(ScopeContext.Element);
            if (index < 0)
            {
                return;
            }

            var rst = itemContext.RemoveScope(ScopeContext.Scope);
            if (rst)
            {
                RemoveScope(index);
                context.IsDirty = true;
            }

            ScopeContext = null;
        }


        private void DrawScopeList(ItemDraftContext draftContext)
        {
            var config = draftContext.ConfigItem;

            var hasScopes = config.Scopes != null && config.Scopes.Count > 0;
            _noneTip.SetDisplay(!hasScopes);
            _scopeListRoot.SetDisplay(hasScopes);

            if (config.Scopes == null)
            {
                return;
            }

            foreach (var scope in config.Scopes)
            {
                var scopeContext = new ScopeContext {Element = _pool.Get(), Scope = scope};
                DrawScope(scopeContext);
            }
        }

        private void DrawScope(ScopeContext context)
        {
            var element = context.Element;
            var tf = element.Q<TextField>();
            tf.value = context.Scope;
            _scopeListRoot.Add(element);

            tf.RegisterCallback<ChangeEvent<string>, ScopeContext>((evt, ctx) =>
                {
                    ctx.Scope = evt.newValue;
                    var rst = itemContext.ModifyScope(evt.previousValue, evt.newValue);
                    if (rst)
                    {
                        this.context.IsDirty = true;
                    }
                },
                context);
            element.RegisterCallback<ClickEvent, ScopeContext>((evt, ctx) => { ScopeContext = ctx; }, context);

            var hasScopes = _scopeListRoot.childCount > 0;
            _noneTip.SetDisplay(!hasScopes);
            _scopeListRoot.SetDisplay(hasScopes);
        }

        private void ClearScopeList()
        {
            var temp = _scopeListRoot;
            while (temp.childCount > 0)
            {
                RemoveScope(0);
            }
        }

        private void RemoveScope(int index)
        {
            var temp = _scopeListRoot;
            var ev = temp.ElementAt(index);
            _pool.Return(ev);
            temp.RemoveAt(index);
        }
    }
}