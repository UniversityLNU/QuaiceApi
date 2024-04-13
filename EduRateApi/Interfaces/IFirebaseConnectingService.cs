using FireSharp;

namespace EduRateApi.Interfaces
{
    public interface IFirebaseConnectingService
    {
        public FirebaseClient GetFirebaseClient();

        public string GetApiKey();
    }
}
