using UEC.UIFramework;

namespace UEC
{
    public class UECContext
    {
        public UECConfigModel UECConfigModel;
        public UPMConfigModel UPMConfigModel;
        public ManifestModel ManifestModel;

        public UECContext()
        {
            UECConfigModel = new UECConfigModel();
            UPMConfigModel = new UPMConfigModel();
            ManifestModel = new ManifestModel();
        }
        
        public ItemDraftContext CurrentSelectItemContext { get; private set; }

        public void SetItemDraftContext(ItemDraftContext itemContext)
        {
            CurrentSelectItemContext = itemContext;
        }
    }
}