using System.Collections.Generic;
using UEC.Event;
using UEC.UIFramework;
using UnityEngine.UIElements;

namespace UEC
{
    public class OperateView : View<UECUI>
    {
        private VisualElementCache _cache;

        private Button revertBtn;
        private Button applyBtn;

        private UECContext context => UI.Context;
        private ItemDraftContext itemContext => context.ItemContext;
        private Dictionary<string, ItemDraftContext> itemContexts => context.ItemContexts;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("operate_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            revertBtn = _cache.Get<Button>("revert_btn");
            revertBtn.clicked += () =>
            {
                RevertChange();
                context.Revert();
                UI.Refresh();
            };

            applyBtn = _cache.Get<Button>("apply_btn");
            applyBtn.clicked += () =>
            {
                if (ApplyChange())
                {
                    context.Apply();
                    UI.Refresh();
                }
            };

            Refresh();

            context.DirtyAction += Refresh;
        }

        private void RevertChange()
        {
            if (context.IsDirty == false)
            {
                return;
            }

            context.SetItemDraftContext(null);
        }

        private bool ApplyChange()
        {
            if (context.IsDirty == false)
            {
                return false;
            }

            if (UI.GetView<TipView>().SaveItemCheck() == false)
            {
                return false;
            }

            if (itemContext != null)
            {
                context.SetItemUsername(itemContext.ConfigItem.Username);
            }

            foreach (var pair in itemContexts)
            {
                if (pair.Value.IsNew)
                {
                    var configItem = pair.Value.ConfigItem;
                    var ret = context.UECConfigModel.AddItem(configItem);
                    if (ret)
                    {
                        pair.Value.IsDirty = false;
                    }
                }
                else if (pair.Value.IsRemove)
                {
                    var configItem = pair.Value.ConfigItem;
                    var ret = context.UECConfigModel.RemoveItem(configItem.Username);
                    if (ret)
                    {
                        pair.Value.IsDirty = false;
                    }
                }
                else
                {
                    var ci = pair.Value.ConfigItem;
                    var ret = context.UECConfigModel.ModifyItem(ci.Username, ci.Token, ci.Scopes);
                    if (ret)
                    {
                        pair.Value.IsDirty = false;
                    }
                }
            }

            context.IsDirty = false;
            return true;
        }

        public void Refresh()
        {
            var isDirty = context.IsDirty;
            applyBtn.SetEnabled(isDirty);
            revertBtn.SetEnabled(isDirty);
        }
    }
}