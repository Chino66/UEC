using System;
using UEC.UIFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UEC
{
    // public enum ItemState
    // {
    //     Default,
    //     Add,
    //     Modify,
    //     Delete
    // }

    public class ItemDraftContext
    {
        public UECContext Context;
        public VisualElement Element;
        public ConfigItem ConfigItem;
        public bool IsDirty;
        public bool IsNew;
        public bool IsRemove;
        // public ItemState State = ItemState.Default;
        public string OriginalUsername;

        public void SetUsername(string username)
        {
            ConfigItem.Username = username;
            IsDirty = true;
            RefreshItem();
        }

        public void SetToken(string token)
        {
            ConfigItem.Token = token;
            IsDirty = true;
            RefreshItem();
        }

        public bool AddScope(string scope)
        {
            var ret = ConfigItem.AddScope(scope);
            IsDirty = true;
            RefreshItem();
            return ret;
        }

        public bool RemoveScope(string scope)
        {
            var ret = ConfigItem.RemoveScope(scope);
            IsDirty = true;
            RefreshItem();
            return ret;
        }

        public bool ModifyScope(string old, string scope)
        {
            var ret = ConfigItem.ModifyScope(old, scope);
            IsDirty = true;
            RefreshItem();
            return ret;
        }

        public void RefreshItem()
        {
            var element = Element;
            var config = ConfigItem;

            var uLab = element.Q<Label>("username");
            var tLab = element.Q<Label>("token");
            var sLab = element.Q<Label>("scopes");

            uLab.text = config.Username;
            tLab.text = config.Token;
            sLab.text = config.GetScopesOverview();

            uLab.style.overflow = Overflow.Hidden;
            tLab.style.overflow = Overflow.Hidden;
            sLab.style.overflow = Overflow.Hidden;

            var dirty = element.Q<Label>("dirty");
            dirty.SetDisplay(IsDirty);
        }
    }
}