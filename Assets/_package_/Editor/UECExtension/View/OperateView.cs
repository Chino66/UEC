using UEC.Event;
using UEC.UIFramework;
using UnityEngine.UIElements;

namespace UEC
{
    public class OperateView : View<UECUI>
    {
        private VisualElementCache _cache;
//        private Button _revertBtn;
//        private Button _applyBtn;

        private ItemDraftContext DraftContext => UI.DraftContext;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("operate_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var revertBtn = _cache.Get<Button>("revert_btn");
            revertBtn.clicked += () =>
            {
                EventCenter.SendEvent("UECConfigModel", "Revert");
                UI.Refresh();
            };

            var applyBtn = _cache.Get<Button>("apply_btn");
            applyBtn.clicked += () =>
            {
                ApplyChange(DraftContext);
                EventCenter.SendEvent("UECConfigModel", "Apply");
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
                var draft = context.ConfigItemDraft;
                var ret = (bool) EventCenter.SendEvent("UECConfigModel", "AddItem",
                    draft.Username, draft.Token, draft.Scopes);
                if (ret)
                {
                    DraftContext.IsDirty = false;
                }
            }
            else
            {
                var draft = context.ConfigItemDraft;
                var ret = (bool) EventCenter.SendEvent("UECConfigModel", "ModifyItem",
                    draft.OriginalUsername, draft.Username, draft.Token, draft.Scopes);
                if (ret)
                {
                    DraftContext.IsDirty = false;
                }
            }
        }

        public void Refresh()
        {
//            var isDirty = UI.UecConfig.IsDirty;
//            _applyBtn.SetEnabled(isDirty);
//            _revertBtn.SetEnabled(isDirty);
        }
    }
}