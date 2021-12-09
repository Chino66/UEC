using UEC.UIFramework;
using UnityEngine.UIElements;

namespace UEC
{
    public class OverviewView : View
    {
        private VisualElementCache _cache;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("list_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);
        }
    }
}