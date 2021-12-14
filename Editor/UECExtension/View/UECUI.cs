using System.IO;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;

namespace UEC
{
    public class UECUI : UI
    {
        public static UECUI CreateUI(VisualElement parent = null)
        {
            var ui = new UECUI();
            if (parent == null)
            {
                parent = new VisualElement();
            }

            ui.Initialize(parent);
            return ui;
        }

        public UECConfigModel UecConfig { get; }
        private UPMConfigModel _upmConfig;
        private ManifestModel _manifest;

        private UECUI()
        {
            UecConfig = new UECConfigModel();
            _upmConfig = new UPMConfigModel();
            _manifest = new ManifestModel();
        }

        protected override void OnInitialize(VisualElement parent)
        {
            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uec_view_uxml.uxml");
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var ussPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uss.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            var temp = asset.CloneTree();
            temp.styleSheets.Add(styleSheet);
            AddStyleSheet(styleSheet);
            Add(temp);

            AddView<OverviewView>();
            AddView<DetailView>();
        }
    }
}