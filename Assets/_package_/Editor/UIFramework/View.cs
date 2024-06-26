using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UEC.UIFramework
{
    public abstract class View //: VisualElement
    {
        public UI UI;

        public VisualElement Parent => this.Parent;
        public VisualElement Self { get; }

        public View()
        {
            Self = new VisualElement();
        }

        public void Add(VisualElement element)
        {
            Self.Add(element);
        }

        public virtual void Initialize(VisualElement parent)
        {
            Self.name = $"{GetType().Name}";
            parent.Add(Self);
            OnInitialize(parent);
        }

        protected virtual void OnInitialize(VisualElement parent)
        {
        }

        public virtual void SetDisplay(bool value)
        {
            Self?.SetDisplay(value);
        }

        public virtual void Show()
        {
            Self?.SetDisplay(true);
        }

        public virtual void Hide()
        {
            Self?.SetDisplay(false);
        }

        public virtual void SetUI(UI ui)
        {
            this.UI = ui;
        }
    }

    public class View<T> : View where T : UI
    {
        public T UI;

        public override void SetUI(UI ui)
        {
            this.UI = (T) ui;
        }
    }
}