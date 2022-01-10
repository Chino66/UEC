namespace UEC
{
    public class ModelCollection
    {
        public UECConfigModel UecConfig { get; }
//        private UPMConfigModel _upmConfig;
//        private ManifestModel _manifest;

        public ModelCollection()
        {
            UecConfig = new UECConfigModel();
//            _upmConfig = new UPMConfigModel();
//            _manifest = new ManifestModel();
        }
    }
}