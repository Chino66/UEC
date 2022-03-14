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

        public Dictionary<string /*username*/, ItemDraftContext> ItemContexts;

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

        public UECContext()
        {
            UECConfigModel = new UECConfigModel();
            UPMConfigModel = new UPMConfigModel();
            ManifestModel = new ManifestModel();

            ItemContexts = new Dictionary<string, ItemDraftContext>();
        }


        public void UpdateDirty()
        {
            var dirty = false;
            foreach (var pair in ItemContexts)
            {
                if (!pair.Value.IsDirty && !pair.Value.IsNew && !pair.Value.IsRemove)
                {
                    continue;
                }

                dirty = true;
                break;
            }

            IsDirty = dirty;
        }

        public string ItemUsername { get; private set; }

        public void SetItemUsername(string username)
        {
            ItemUsername = username;
        }


        public ItemDraftContext ItemContext { get; private set; }

        public void SetItemDraftContext(ItemDraftContext itemContext)
        {
            ItemContext = itemContext;
        }

        public void AddItem(ItemDraftContext context)
        {
            ItemContexts.Add(context.ConfigItem.Username, context);
            context.IsNew = true;
            IsDirty = true;
        }

        public void RemoveItem(string username)
        {
            if (ItemContexts.TryGetValue(username, out var context))
            {
                if (context.IsNew)
                {
                    ItemContexts.Remove(username);
                }
                else
                {
                    context.IsRemove = true;
                    IsDirty = true;
                }
            }
        }


        public string GetUsername()
        {
            return ItemContext.ConfigItem.Username;
        }

        public void SetUsername(string username)
        {
            ItemContext.SetUsername(username);
            IsDirty = true;
        }

        public string GetToken()
        {
            return ItemContext.ConfigItem.Token;
        }

        public void SetToken(string token)
        {
            ItemContext.SetToken(token);
            IsDirty = true;
        }

        public bool AddScope(string scope)
        {
            var ret = ItemContext.AddScope(scope);
            if (ret)
            {
                IsDirty = true;
            }
            return ret;
        }

        public bool RemoveScope(string scope)
        {
            var ret = ItemContext.RemoveScope(scope);
            if (ret)
            {
                IsDirty = true;
            }
            return ret;
        }

        public bool ModifyScope(string old, string scope)
        {
            var ret = ItemContext.ModifyScope(old, scope);
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