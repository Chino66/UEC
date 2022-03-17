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

        private Button _removeBtn;

        private VisualElement _tokenBox;

        private Toggle _toggle;

        private ScopeContext ScopeContext
        {
            get { return _scopeContext; }

            set
            {
                _removeBtn.SetEnabled(value != null);

                _scopeContext = value;
            }
        }

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
                if (context.GetUsername() == evt.newValue)
                {
                    return;
                }

                context.SetUsername(evt.newValue);
            });

            _tokenTf = _cache.Get<TextField>("token_tf");
            _tokenTf.RegisterValueChangedCallback(evt =>
            {
                if (context.GetToken() == evt.newValue)
                {
                    return;
                }

                context.SetToken(evt.newValue);
            });

            _tokenBox = _cache.Get<VisualElement>("token_box");
            _tokenBox.SetDisplay(false);

            _toggle = _cache.Get<Toggle>("develop_toggle");
            _toggle.RegisterValueChangedCallback(evt =>
            {
                if (context.GetIsDeveloper() != evt.newValue)
                {
                    context.SetIsDeveloper(evt.newValue);
                }

                _tokenBox.SetDisplay(evt.newValue);
            });

            _scopeListRoot = _cache.Get("scope_item_list_root");
            _noneTip = _cache.Get("none_tip");

            var addBtn = _cache.Get<Button>("add_item_btn");
            addBtn.clicked += () =>
            {
                ScopeContext = new ScopeContext {Element = _pool.Get(), Scope = "*"};
                var rst = context.AddScope(ScopeContext.Scope);
                if (rst)
                {
                    DrawScope(ScopeContext);
                }
            };

            _removeBtn = _cache.Get<Button>("remove_item_btn");
            _removeBtn.clicked += () =>
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

                var rst = context.RemoveScope(ScopeContext.Scope);
                if (rst)
                {
                    RemoveScope(index);
                }

                ScopeContext = null;
            };

            var usernameTip = _cache.Get<Button>("username_tip");
            usernameTip.tooltip = "Your or other's Github account name";

            var tokenTip = _cache.Get<Button>("token_tip");
            tokenTip.tooltip = "Credentials obtained from github, click the button for details";
            tokenTip.clicked += () =>
            {
                const string url =
                    "https://docs.github.com/cn/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token";
                Application.OpenURL(url);
            };

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
            _toggle.value = config.IsDeveloper;

            ClearScopeList();
            DrawScopeList(draftContext);

            Show();
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

        private void DrawScope(ScopeContext scopeContext)
        {
            var element = scopeContext.Element;
            var tf = element.Q<TextField>();
            tf.value = scopeContext.Scope;
            _scopeListRoot.Add(element);

            tf.RegisterCallback<ChangeEvent<string>, ScopeContext>((evt, ctx) =>
                {
                    ctx.Scope = evt.newValue;
                    context.ModifyScope(evt.previousValue, evt.newValue);
                },
                scopeContext);

            element.RegisterCallback<ClickEvent, ScopeContext>((evt, ctx) => { ScopeContext = ctx; }, scopeContext);

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