namespace TravelGroupProject.Models
{
    public class ApiResponse
    {
        private bool success;
        private int packageId;

        public bool Success
        {
            get {  return success; } 
            set { success = value; }
        }

        public int PackageId
        {
            get { return packageId; }
            set { packageId = value; }
        }
    }
}
