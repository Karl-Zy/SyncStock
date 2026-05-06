namespace SyncStock.Models
{
    public class AssetManager : Users
    {
        public AssetManager(string username, string passwordHash, string department)
            : base(username, passwordHash, department)
        {
            Role = UserRole.AssetManager;
        }

        public override bool CanAccessFeature(string featureName)
        {
            // Asset Manager has master access, including the Unlock capability
            return true;
        }
    }
}