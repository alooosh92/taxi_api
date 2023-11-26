using Microsoft.AspNetCore.Mvc;
using taxi_api.Models;
using taxi_api.VModels;

namespace taxi_api.Data
{
    public interface  IRepository
    {
        //---ConnectWithUs---
        public Task<ActionResult<bool>> AddConnectWithUs(string userid,ConnectWithUsVM connectWithUsVM);
        //----Driver---
        public Task<ActionResult<bool>> AddDriver(DriverVM driver);
        public Task<ActionResult<bool>> UpdateDriver(Guid id,DriverVM driver);
        public Task<ActionResult<bool>> DeleteDriver(Guid id);
        public Task<ActionResult<bool>> ChangeStateDriver(string driverId, bool state);
        public Task<ActionResult<bool>> AddBalanceDriver(Guid id, double balance);
        public Task<ActionResult<DriverVM>?> GetDriver(Guid id);
        //---location---
        public Task<ActionResult<bool>> AddUserLocation(string userId,UserLocationVM userLocation);
        public Task<ActionResult<bool>> DeleteUserLocation(Guid locationId);
        public Task<ActionResult<List<ShowUserLocationVM>>> GetUserLocations(string userId);
        //---TermsOfUseAndPrivacy---
        public Task<ActionResult<bool>> AddTermsOfUseAndPrivacy(PrivacyAndTermVM privacy);
        public Task<ActionResult<bool>> UpdateTermsOfUseAndPrivacy(Guid id, PrivacyAndTermVM privacy);
        public Task<ActionResult<bool>> DeleteTermsOfUseAndPrivacy(Guid id);
        public Task<ActionResult<List<TermsOfUseAndPrivacy>>> GetTermsOfUseAndPrivacy(bool isPrivacy);
        //---Trip---
        public Task<ActionResult<bool>> AddTrip(string userId,TripVM trip);
        public Task<ActionResult<bool>> DeleteTrip(string userId, Guid id);
        public Task<ActionResult<bool>> AcceptedTrip(Guid id,string driverId);
        public Task<ActionResult<bool>> EndedTrip(Guid id);
        public Task<ActionResult<List<ShowTripVM>>> GetAllTrip(string phone);
        public Task<ActionResult<ShowTripVM>?> GetTrip(Guid id);
        public Task<ActionResult<List<TripForWebSocketVW>>> GetAllTripForDriver(string userId,double lat, double lon);
        public Task<ActionResult<List<TripForUser>>> GetAllTripForUser(string userId);
        //---User---
        public Task<ActionResult<RegisterVM>?> UpdateUserInfo(string userId,RegisterVM info);
        public Task<ActionResult<RegisterVM>?> GetUserInfo(string userId);
    }
}
