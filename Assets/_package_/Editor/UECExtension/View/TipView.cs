using System.Collections.Generic;
using System.Linq;
using UEC.Event;
using UEC.UIFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UEC
{
    public class TipView : View<UECUI>
    {
        private VisualElementCache _cache;

        private Label _tip;

        private UECContext context => UI.Context;
        private ItemDraftContext currentSelectItemContext => context.ItemContext;
        private Dictionary<string, ItemDraftContext> ItemContexts => context.ItemContexts;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("tip_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            _tip = _cache.Get<Label>("tip");
            Refresh();
        }

        public void Refresh(string msg = null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                _tip.SetDisplay(false);
            }
            else
            {
                _tip.SetDisplay(true);
                _tip.text = msg;
            }
        }

        public void Error(string msg)
        {
            _tip.style.color = new StyleColor(Color.red);
            Refresh(msg);
        }

        public bool AddItemCheck()
        {
            if (currentSelectItemContext == null)
            {
                return true;
            }

            if (currentSelectItemContext.IsDirty == false)
            {
                Error("Modifications are not saved");
                return false;
            }

            if (currentSelectItemContext.IsNew == true)
            {
                Error("New item are not saved ");
                return false;
            }

            return true;
        }

        public bool SaveItemCheck()
        {
            var sameAges = ItemContexts.GroupBy(g => g.Value.ConfigItem.Username).Where(s => s.Count() > 1).ToList();
            if (sameAges.Count > 0)
            {
                Error($"Username {sameAges[0].Key} is repeat");
                return false;
            }

            if (ItemContexts.Any(pair => string.IsNullOrEmpty(pair.Value.ConfigItem.Username)))
            {
                Error($"Username can not be empty!");
                return false;
            }

            if (ItemContexts.Any(pair => string.IsNullOrEmpty(pair.Value.ConfigItem.Token)))
            {
                Error($"Token can not be empty!");
                return false;
            }

            if (ItemContexts.Any(pair => pair.Value.ConfigItem.Scopes.Any(scope => string.IsNullOrEmpty(scope))))
            {
                Error($"Scope can not be empty!");
                return false;
            }

            return true;
        }
    }
}