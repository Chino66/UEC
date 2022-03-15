using System.Collections.Generic;
using System.Linq;
using UEC.Event;
using UEC.UIFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UEC
{
    public class UpdateView : View<UECUI>
    {
        private VisualElementCache _cache;

        // private UECContext context => UI.Context;
        // private ItemDraftContext currentSelectItemContext => context.ItemContext;
        // private Dictionary<string, ItemDraftContext> ItemContexts => context.ItemContexts;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("update_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var btn = _cache.Get<Button>("update_btn");
            btn.clicked += () => { UI.Context.ManifestModel.Test(); };
        }
    }
}