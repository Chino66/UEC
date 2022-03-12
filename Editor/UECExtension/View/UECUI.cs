using System.IO;
using UEC.Event;
using UEC.UIFramework;
using UnityEditor;
using UnityEngine;
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
        
        public UECContext Context;

        private UECUI()
        {
            Context = new UECContext();
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
            AddView<OperateView>();
        }

        public void Refresh()
        {
            GetView<OverviewView>().Refresh();
            GetView<DetailView>().Refresh();
            GetView<OperateView>().Refresh();
        }

        // public void SetDraftContext(ItemDraftContext context)
        // {
        //     DraftContext = context;
        // }
    }
}