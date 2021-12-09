using UEC.UIFramework;
using UnityEngine.UIElements;

namespace UEC
{
    public class DetailView: View
    {
        private VisualElementCache _cache;
        
        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("detail_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);
        }

    }
}