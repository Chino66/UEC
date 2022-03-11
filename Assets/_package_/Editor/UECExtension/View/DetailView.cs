using System.IO;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class ScopeContext
    {
        public string Scope;
        public VisualElement Element;
    }

    public class DetailView : View<UECUI>
    {
        private VisualElementCache _cache;
        private VisualElementPool _pool;
        
        private TextField _nameTf;
        private TextField _tokenTf;
        
        private VisualElement _scopeListRoot;
        private VisualElement _noneTip;

//        private VisualElement _selectedScope;
        private ScopeContext _selecedScopeContext;

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
                // todo name check
                UI.GetView<OverviewView>().SetUsername(evt.newValue);
            });
            _tokenTf = _cache.Get<TextField>("token_tf");
            _tokenTf.RegisterValueChangedCallback(evt =>
            {
                // todo token check
                UI.GetView<OverviewView>().SetToken(evt.newValue);
            });

            _scopeListRoot = _cache.Get("scope_item_list_root");
            _noneTip = _cache.Get("none_tip");

            var addBtn = _cache.Get<Button>("add_item_btn");
            addBtn.clicked += () => { AddScope(); };

            var removeBtn = _cache.Get<Button>("remove_item_btn");
            removeBtn.clicked += () => { RemoveSelectScope(); };

            Hide();
        }

        public void Refresh(ItemContext context)
        {
            if (context == null)
            {
                Hide();
                return;
            }

            ShowItemDetail(context);
        }

        private void AddScope()
        {
            var scopeContext = new ScopeContext {Element = _pool.Get(), Scope = "*"};
            _selecedScopeContext = scopeContext;
            UI.GetView<OverviewView>().AddScope(_selecedScopeContext.Scope);
            DrawScope(scopeContext);
        }

        private void RemoveSelectScope()
        {
            if (_selecedScopeContext == null)
            {
                return;
            }

            var index = _scopeListRoot.IndexOf(_selecedScopeContext.Element);
            if (index < 0)
            {
                return;
            }

            UI.GetView<OverviewView>().RemoveScope(_selecedScopeContext.Scope);
            RemoveScope(index);

            _selecedScopeContext = null;
        }

        public void ShowItemDetail(ItemContext context)
        {
            var config = context.ConfigItem;
            _nameTf.value = config.Username;
            _tokenTf.value = config.Token;

            ClearScopeList();
            DrawScopeList(context);

            Show();
        }

        private void DrawScopeList(ItemContext context)
        {
            var config = context.ConfigItem;

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
                    UI.GetView<OverviewView>().ModifyScope(evt.previousValue, evt.newValue);
                },
                context);
            element.RegisterCallback<ClickEvent, ScopeContext>((evt, ctx) => { _selecedScopeContext = ctx; }, context);
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