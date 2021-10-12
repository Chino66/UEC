using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UPMEnvironmentConfigure;
using Button = UnityEngine.UIElements.Button;

namespace UEC
{
    public class UPMExtensionUI : VisualElement
    {
        public static UPMExtensionUI CreateUI()
        {
            return new UPMExtensionUI();
        }

        private VisualElement _root;

        private UPMExtensionUI()
        {
            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uec_uxml.uxml");
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var root = asset.CloneTree();
            var ussPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uss.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            root.styleSheets.Add(styleSheet);
            Add(root);
            
            

            // 依赖项UI模板
//            uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/dependencies_item_uxml.uxml");
//            _dependenciesItemVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

//            _dependenciesItemQueue = new Queue<VisualElement>();
        }


        private void ShowPath(Environment.SpecialFolder folder)
        {
            var path = UPMConfig.SpecialFolder(folder);

            Debug.Log($"Environment.SpecialFolder is {folder}, path is {path}");
        }

        private VisualElement _packageVersionRoot;

        private List<string> _tagsList;

        private Button _getGitTagsButton;
        private PopupField<string> _versionTagsPopupField;
        private Button _changeVersionButton;
        private VisualTreeAsset _dependenciesItemVisualTreeAsset;

        private Queue<VisualElement> _dependenciesItemQueue;

        private void InitPackageVersion()
        {
            _packageVersionRoot = new VisualElement {name = "package_version_root"};
            Add(_packageVersionRoot);

            // 插件版本UI用代码生成,不用uxml的原因是PopupField组件不能用uxml
            _getGitTagsButton = new Button {name = "get_git_tags", text = "获取版本信息"};
            _packageVersionRoot.Add(_getGitTagsButton);

            _tagsList = new List<string> {"-select version-"};
            _versionTagsPopupField = new PopupField<string>("Version:", _tagsList, 0) {value = "-select version-"};
            _versionTagsPopupField.SetEnabled(false);
            _packageVersionRoot.Add(_versionTagsPopupField);

            _changeVersionButton = new Button {name = "change_version", text = "切换版本"};
            _changeVersionButton.SetEnabled(false);
            _packageVersionRoot.Add(_changeVersionButton);
        }
    }
}