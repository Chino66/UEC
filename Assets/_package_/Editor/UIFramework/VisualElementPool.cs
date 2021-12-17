using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UEC.UIFramework
{
    public class VisualElementPool : Pool<VisualElement>
    {
        private readonly VisualTreeAsset _asset;

        protected Func<VisualElement, VisualElement> CreateFunc;

        public VisualElementPool(VisualTreeAsset asset)
        {
            _asset = asset;
        }

        public VisualElementPool SetCreateFunc(Func<VisualElement, VisualElement> func)
        {
            CreateFunc = func;
            return this;
        }

        protected override VisualElement Create()
        {
            var o = _asset.CloneTree();
            var item = CreateFunc != null ? CreateFunc.Invoke(o) : o;
            return item;
        }

        public override void Return(VisualElement item, bool checkContains = false)
        {
            item.Unbind();
            base.Return(item, checkContains);
        }
    }
}