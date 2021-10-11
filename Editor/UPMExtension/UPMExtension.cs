using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UEC
{
    [InitializeOnLoad]
    public class UPMExtension : IPackageManagerExtension
    {
        static UPMExtension()
        {
            PackageManagerExtensions.RegisterExtension(new UPMExtension());
        }
        
        private UPMExtensionUI _ui;

        public VisualElement CreateExtensionUI()
        {
            if (_ui == null)
            {
                _ui = UPMExtensionUI.CreateUI();
            }

            return _ui;
        }

        public void OnPackageSelectionChange(PackageInfo packageInfo)
        {
        }

        public void OnPackageAddedOrUpdated(PackageInfo packageInfo)
        {
        }

        public void OnPackageRemoved(PackageInfo packageInfo)
        {
        }
    }
}