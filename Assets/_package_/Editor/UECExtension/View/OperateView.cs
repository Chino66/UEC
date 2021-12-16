using UEC.UIFramework;
using UnityEngine.UIElements;

namespace UEC
{
    public class OperateView : View<UECUI>
    {
        private VisualElementCache _cache;
        private Button _revertBtn;
        private Button _applyBtn;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("operate_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            _revertBtn = temp.Q<Button>("revert_btn");
            _revertBtn.clicked += () =>
            {
                UI.UecConfig.Revert();
                UI.Refresh();
            };

            _applyBtn = temp.Q<Button>("apply_btn");
            _applyBtn.clicked += () =>
            {
                UI.UecConfig.Apply();
                UI.Refresh();
            };

            Refresh();
        }

        public void Refresh()
        {
            var isDirty = UI.UecConfig.IsDirty;
            _applyBtn.SetEnabled(isDirty);
            _revertBtn.SetEnabled(isDirty);
        }
    }
}