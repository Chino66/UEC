using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
            _root = new VisualElement {name = "root"};
            Add(_root);

            var box = new Box();
            box.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            _root.Add(box);
            {
                var textField = new TextField("Username");
                textField.value = "Enter Github Username";
                box.Add(textField);

                textField = new TextField("Token");
                textField.value = "Enter Github Token";
                box.Add(textField);
            }


            var btn = new Button();
            btn.text = "show path";
            _root.Add(btn);
            btn.clickable.clicked += () =>
            {
                var lines = File.ReadAllLines(UPMConfig.UPMConfigPath);

                foreach (var line in lines)
                {
                    Debug.Log(line);
                }
            };
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