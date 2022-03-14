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
            var context = currentSelectItemContext;

            if (!CheckValid(context.ConfigItem.Username))
            {
                Error($"Username {context.ConfigItem.Username} is invalid");
                return false;
            }

            if (!CheckValid(context.ConfigItem.Token))
            {
                Error($"Token {context.ConfigItem.Token} is invalid");
                return false;
            }

            foreach (var scope in context.ConfigItem.Scopes)
            {
                if (!CheckValid(scope))
                {
                    Error($"Scope {scope} is invalid");
                    return false;
                }
            }

            var sameAges = context.ConfigItem.Scopes.GroupBy(g => g).Where(s => s.Count() > 1).ToList();
            if (sameAges.Count > 0)
            {
                Error($"Scope is repeat");
                return false;
            }

            return true;
        }

        private bool CheckValid(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            return true;
        }
    }
}