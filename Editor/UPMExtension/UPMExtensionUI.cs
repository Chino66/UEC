using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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
            // _root = new VisualElement {name = "root"};
            // Add(_root);
            //
            // var lab = new Label();
            // lab.text = "sssssssssssssssssssss";
            // _root.Add(lab);
            //
            //
            // var btn = new Button();
            // btn.text = "ssss";
            // _root.Add(btn);
            InitPackageVersion();
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