using UEC.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;

namespace UEC
{
    public class UpdateView : View<UECUI>
    {
        private VisualElementCache _cache;
        private UECContext context => UI.Context;
        private Button updateBtn;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("update_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            updateBtn = _cache.Get<Button>("update_btn");
            updateBtn.clicked += () =>
            {
                context.ManifestModel.Update();
                context.UPMConfigModel.Update();
                context.NpmrcModel.Update();
            };
            Refresh();
            context.DirtyAction += Refresh;

            var view = _cache.Get<Button>("view_manifest");
            view.clicked += () => { EditorUtility.RevealInFinder(ManifestModel.ManifestPath); };

            view = _cache.Get<Button>("view_upmconfig");
            view.clicked += () => { EditorUtility.RevealInFinder(UPMConfigModel.UPMConfigPath); };

            view = _cache.Get<Button>("view_npmrc");
            view.clicked += () => { EditorUtility.RevealInFinder(NpmrcModel.NpmrcPath); };

            // var btn = new Button();
            // updateBtn.parent.Add(btn);
            // btn.text = "upmconfig";
            // btn.clicked += () => { context.UPMConfigModel.Update(); };
        }

        public void Refresh()
        {
            var isDirty = context.IsDirty;
            updateBtn.SetEnabled(!isDirty);
        }
    }
}