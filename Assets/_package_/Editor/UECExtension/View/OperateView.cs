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
                context.UECConfigModel.Revert();
                UI.Refresh();
            };

            applyBtn = _cache.Get<Button>("apply_btn");
            applyBtn.clicked += () =>
            {
                ApplyChange(currentSelectItemContext);
                this.context.UECConfigModel.Apply();
                UI.Refresh();
            };

            Refresh();
        }

        private void ApplyChange(ItemDraftContext context)
        {
            if (context == null || context.IsDirty == false)
            {
                return;
            }

            if (context.IsNew)
            {
                var draft = context.ConfigItem;
                var ret = this.context.UECConfigModel.AddItem(draft);
                if (ret)
                {
                    context.IsDirty = false;
                }
            }
            else
            {
                var draft = context.ConfigItem;
                var ret = this.context.UECConfigModel.ModifyItem(context.OriginalUsername, draft.Username, draft.Token, draft.Scopes);
                if (ret)
                {
                    context.OriginalUsername = draft.Username;
                    context.IsDirty = false;
                }
            }
        }

        public void Refresh()
        {
            // var isDirty = context.UECConfigModel.IsDirty;
            // applyBtn.SetEnabled(isDirty);
            // revertBtn.SetEnabled(isDirty);
        }
    }
}