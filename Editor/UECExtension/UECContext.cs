using System;
using System.Collections.Generic;
using UEC.UIFramework;

namespace UEC
{
    public class UECContext
    {
        public UECConfigModel UECConfigModel;
        public UPMConfigModel UPMConfigModel;
        public ManifestModel ManifestModel;

        public Dictionary<string /*username*/, ItemDraftContext> ItemDraftContexts;

        public UECContext()
        {
            UECConfigModel = new UECConfigModel();
            UPMConfigModel = new UPMConfigModel();
            ManifestModel = new ManifestModel();

            ItemDraftContexts = new Dictionary<string, ItemDraftContext>();
        }


        public Action DirtyAction;

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                DirtyAction?.Invoke();
            }
        }

        public string CurrentItemConfigUsername { get; private set; }

        public void SetCurrentItemConfigUsername(string username)
        {
            CurrentItemConfigUsername = username;
        }


        public ItemDraftContext CurrentSelectItemContext { get; private set; }

        public void SetItemDraftContext(ItemDraftContext itemContext)
        {
            CurrentSelectItemContext = itemContext;
        }

        public bool RemoveItem(string username)
        {
            var ret = UECConfigModel.RemoveItem(username);

            if (ret)
            {
                IsDirty = true;
            }

            return ret;
        }

        public void Apply()
        {
            UECConfigModel.Apply();
            IsDirty = false;
        }

        public void Revert()
        {
            UECConfigModel.Revert();
            IsDirty = false;
        }
    }
}