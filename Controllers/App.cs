using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taxi_api.Data;
using taxi_api.Models;
using taxi_api.VModels;

namespace taxi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class App : ControllerBase
    {
        public App(
            IRepository _repository, UserManager<IdentityUser> _userManager)
        {
            Repository = _repository;
            UserManager = _userManager;
        }
        public IRepository Repository { get; }
        public UserManager<IdentityUser> UserManager { get; }
        //---ConnectWithUs---
        [HttpPost]
        [Route("AddConnectWithUs")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> AddConnectWithUs([FromBody] ConnectWithUsVM connectWithUsVM)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                var user = UserManager.GetUserId(User);
                return await Repository.AddConnectWithUs(user!, connectWithUsVM);
            }
            catch { throw; }
        }
        //----Driver---
        [HttpPost]
        [Route("AddDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> AddDriver(DriverVM driver)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.AddDriver(driver);
            }
            catch { throw; }
        }
        [HttpPut]
        [Route("UpdateDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Driver")]
        public async Task<ActionResult<bool>> UpdateDriver(Guid id, [FromBody] DriverVM driver)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.UpdateDriver(id, driver);
            }
            catch { throw; }

        }
        [HttpDelete]
        [Route("DeleteDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteDriver(Guid id)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.DeleteDriver(id);
            }
            catch { throw; }
        }
        [HttpPost]
        [Route("ChangeStateDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver")]
        public async Task<ActionResult<bool>> ChangeStateDriver(bool state)
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.ChangeStateDriver(u!, state);
            }
            catch { throw; }
        }
        [HttpPost]
        [Route("AddBalanceDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> AddBalanceDriver(Guid id, double balance)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.AddBalanceDriver(id, balance);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetDriver")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<DriverVM>?> GetDriver(Guid id)
        {
            try
            {
                return await Repository.GetDriver(id);
            }
            catch { throw; }
        }
        //---location---
        [HttpPost]
        [Route("AddUserLocation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> AddUserLocation([FromBody] UserLocationVM userLocation)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                var u = UserManager.GetUserId(User);
                return await Repository.AddUserLocation(u!, userLocation);
            }
            catch { throw; }
        }
        [HttpDelete]
        [Route("DeleteUserLocation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> DeleteUserLocation(Guid locationId)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.DeleteUserLocation(locationId);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetUserLocations")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<List<ShowUserLocationVM>>> GetUserLocations()
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.GetUserLocations(u);
            }
            catch { throw; }
        }
        //---TermsOfUseAndPrivacy---
        [HttpPost]
        [Route("AddTermsOfUseAndPrivacy")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> AddTermsOfUseAndPrivacy([FromBody] PrivacyAndTermVM privacy)
        {
            try
            {
                return await Repository.AddTermsOfUseAndPrivacy(privacy);
            }
            catch { throw; }
        }
        [HttpPut]
        [Route("UpdateTermsOfUseAndPrivacy")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateTermsOfUseAndPrivacy(Guid id, PrivacyAndTermVM privacy)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                return await Repository.UpdateTermsOfUseAndPrivacy(id, privacy);
            }
            catch { throw; }
        }
        [HttpDelete]
        [Route("DeleteTermsOfUseAndPrivacy")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteTermsOfUseAndPrivacy(Guid id)
        {
            try
            {
                return await Repository.DeleteTermsOfUseAndPrivacy(id);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetTermsOfUseAndPrivacy")]
        public async Task<ActionResult<List<TermsOfUseAndPrivacy>>> GetTermsOfUseAndPrivacy(bool isPrivacy)
        {
            try
            {
                return await Repository.GetTermsOfUseAndPrivacy(isPrivacy);
            }
            catch { throw; }
        }
        //---Trip---
        [HttpPost]
        [Route("AddTrip")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> AddTrip([FromBody] TripVM trip)
        {
            try
            {
                if (!ModelState.IsValid) { return false; }
                var u = UserManager.GetUserId(User);
                return await Repository.AddTrip(u!, trip);
            }
            catch { throw; }
        }
        [HttpDelete]
        [Route("DeleteTrip")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<bool>> DeleteTrip(Guid id)
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.DeleteTrip(u!, id);
            }
            catch { throw; }
        }
        [HttpPut]
        [Route("AcceptedTrip")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver")]
        public async Task<ActionResult<bool>> AcceptedTrip(Guid id)
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.AcceptedTrip(id, u!);
            }
            catch { throw; }
        }
        [HttpPut]
        [Route("EndedTrip")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver,Admin")]
        public async Task<ActionResult<bool>> EndedTrip(Guid id)
        {
            try
            {
                return await Repository.EndedTrip(id);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetAllTrip")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver,Admin")]
        public async Task<ActionResult<List<ShowTripVM>>> GetAllTrip()
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.GetAllTrip(u!);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetAllTripForDriver")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver,Admin")]
        public async Task<ActionResult<List<TripForWebSocketVW>>> GetAllTripForDriver(double lat, double log)
        {
            try
            {
                var u = UserManager.GetUserId(User);
                return await Repository.GetAllTripForDriver(u!,lat, log);
            }
            catch { throw; }
        }
        [HttpGet]
        [Route("GetAllTripForUser")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<List<TripForUser>>?> GetAllTripForUser()
        {
            var u = UserManager.GetUserId(User);
            return await Repository.GetAllTripForUser(u!);
        }
        [HttpGet]
        [Route("GetTrip")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Driver,Admin")]
        public async Task<ActionResult<ShowTripVM>?> GetTrip(Guid id)
        {
            try
            {
                return await Repository.GetTrip(id);
            }
            catch { throw; }
        }
        [HttpPost]
        [Route("UpdateUserInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<RegisterVM>?> UpdateUserInfo([FromBody] RegisterVM info)
        {
            var u = UserManager.GetUserId(User);
            return await Repository.UpdateUserInfo(u!, info);
        }
        [HttpGet]
        [Route("GetUserInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<RegisterVM>?> GetUserInfo()
        {
            var u = UserManager.GetUserId(User);
            return await Repository.GetUserInfo(u!);
        }        
    }
}
