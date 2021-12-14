using System.IO;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class DetailView : View<UECUI>
    {
        private VisualElementCache _cache;
        private VisualElementPool _pool;
        private TextField _nameTf;
        private TextField _tokenTf;
        private VisualElement _scopeListRoot;
        private VisualElement _noneTip;

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
            _tokenTf = _cache.Get<TextField>("token_tf");
            _scopeListRoot = _cache.Get("scope_item_list_root");
            _noneTip = _cache.Get("none_tip");
            Hide();
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
                var element = _pool.Get();
                element.Q<TextField>().value = scope;
                _scopeListRoot.Add(element);
            }
        }

        private void ClearScopeList()
        {
            var temp = _scopeListRoot;
            while (temp.childCount > 0)
            {
                var ev = temp.ElementAt(0);
                _pool.Return(ev);
                temp.RemoveAt(0);
            }
        }
    }
}