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
        private ItemDraftContext currentSelectItemContext => context.CurrentSelectItemContext;

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
                if (!ApplyChange())
                {
                    return;
                }

                context.Apply();
                UI.Refresh();
            };

            Refresh();

            context.DirtyAction += Refresh;
        }

        private void RevertChange()
        {
            if (currentSelectItemContext == null || this.context.IsDirty == false)
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
            
            if (currentSelectItemContext == null)
            {
                return false;
            }

            if (UI.GetView<TipView>().SaveItemCheck() == false)
            {
                return false;
            }

            context.SetCurrentItemConfigUsername(currentSelectItemContext.ConfigItem.Username);

            if (currentSelectItemContext.IsNew)
            {
                var configItem = currentSelectItemContext.ConfigItem;
                var ret = context.UECConfigModel.AddItem(configItem);
                if (ret)
                {
                    context.IsDirty = false;
                }
            }
            else
            {
                var configItem = currentSelectItemContext.ConfigItem;
                var ret = this.context.UECConfigModel.ModifyItem(configItem.Username, configItem.Token,
                    configItem.Scopes);
                if (ret)
                {
                    context.IsDirty = false;
                }
            }

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