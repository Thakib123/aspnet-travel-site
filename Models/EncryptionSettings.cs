namespace TravelGroupProject.Models
{
    public class EncryptionSettings
    {
        private string key;
        private string iv;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public string IV
        {
            get { return iv; }
            set { iv = value; }
        }
    }
}
