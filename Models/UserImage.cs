namespace TravelGroupProject.Models
{
    public class UserImage
    {
        private int userId;
        private string imageUrl;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
    }
}
