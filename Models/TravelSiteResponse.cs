namespace TravelGroupProject.Models
{
    public class TravelSiteResponse
    {
        private bool success;
        private TravelSiteToken travelSite;

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public TravelSiteToken TravelSite
        {
            get { return travelSite; }
            set { travelSite = value; }
        }
    }
}
