namespace TravelGroupProject.Models
{
    public class TravelSiteToken
    {
        private int travelSiteId;
        private string siteName;
        private string apiToken;

        public TravelSiteToken() { }

        public int TravelSiteId
        {
            get { return travelSiteId; }
            set { travelSiteId = value; }
        }

        public string SiteName
        {
            get { return siteName; }
            set { siteName = value; }
        }

        public string ApiToken
        {
            get { return apiToken; }
            set { apiToken = value; }
        }
    }
}
