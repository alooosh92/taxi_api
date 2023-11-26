using GoogleMaps.LocationServices;
using jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taxi_api.Models;
using taxi_api.VModels;

namespace taxi_api.Data
{
    public class Repository : IRepository
    {
        public Repository(ApplicationDbContext _DB, AuthServies _auth, UserManager<IdentityUser> _userManager)
        {
            DB = _DB;
            Auth = _auth;
            UserManager = _userManager;
        }

        public ApplicationDbContext DB { get; }
        public AuthServies Auth { get; }
        public UserManager<IdentityUser> UserManager { get; }

        public async Task<ActionResult<bool>> AcceptedTrip(Guid id, string driverId)
        {
            try
            {
                Trip? trip = await DB.Trips.FindAsync(id);
                Driver? driv = await DB.Drivers.Include(a => a.IdentityUser).SingleOrDefaultAsync(a => a.IdentityUser!.PhoneNumber == driverId);
                if (trip == null || driv == null) { return false; }
                trip.Accepted = DateTime.Now;
                trip.Drive = driv;
                driv.IsEmpty = false;
                DB.Drivers.Update(driv);
                DB.Trips.Update(trip);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> AddBalanceDriver(Guid id, double balance)
        {
            try
            {
                Driver? driver = await DB.Drivers.FindAsync(id);
                if (driver == null) { return false; }
                driver.Balance += balance;
                DB.Drivers.Update(driver);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> AddConnectWithUs(string userid, ConnectWithUsVM connectWithUsVM)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userid);
                if (user == null) { return false; }
                ConnectWithUs connect = new ConnectWithUs
                {
                    Body = connectWithUsVM.Body,
                    Subject = connectWithUsVM.Subject,
                    IdentityUser = user,
                };
                await DB.ConnectWithUs.AddAsync(connect);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> AddDriver(DriverVM driver)
        {
            try
            {
                Driver driv = new Driver
                {
                    Balance = driver.Balance,
                    CarColor = driver.CarColor,
                    CarNumber = driver.CarNumber,
                    CarType = driver.CarType,
                    FatherName = driver.FatherName,
                    FirtName = driver.FirtName,
                    LastName = driver.LastName,
                    LicenType = driver.LicenType,                    
                };
                bool b = Auth.Register(new UserModel { Email = driver.Phone, Phone = driver.Phone, Name = driver.Phone }).Result.Value;
                if (!b) { return false; }
                var u = await UserManager.FindByNameAsync(driver.Phone!);
                driv.IdentityUser = u!;
                await UserManager.RemoveFromRoleAsync(u!, "User");
                await UserManager.AddToRoleAsync(u!, "Driver");
                await DB.Drivers.AddAsync(driv);
                await DB.SaveChangesAsync();
                return true;
            }
            catch {
                var u = await UserManager.FindByNameAsync(driver.Phone!);
                await UserManager.DeleteAsync(u!);
                throw; 
            }
        }

        public async Task<ActionResult<bool>> AddTermsOfUseAndPrivacy(PrivacyAndTermVM privacy)
        {
            try
            {
                TermsOfUseAndPrivacy termsOfUseAndPrivacy = new TermsOfUseAndPrivacy
                {
                    isPrivacy = privacy.IsPrivacy,
                    Text = privacy.Text,
                    Title = privacy.Title,
                };
                await DB.TermsOfUseAndPrivacy.AddAsync(termsOfUseAndPrivacy);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> AddTrip(string userId, TripVM trip)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
                if (user == null) { return false; }
                if (DB.Trips.Any(a=>a.User == user && a.Ended == null)){ return false; }
                Trip tr = new Trip
                {
                    FromLate = trip.FromLate,
                    FromLong = trip.FromLong,
                    Price = trip.Price,
                    ToLate = trip.ToLate,
                    ToLong = trip.ToLong,
                    User = user,
                };
                await DB.Trips.AddAsync(tr);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> AddUserLocation(string userId, UserLocationVM userLocation)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
                if (user == null) { return false; }
                var loc = new Location
                {
                    Late = userLocation.Late,
                    Long = userLocation.Long,
                    Name = userLocation.Name,
                    User = user
                };
                await DB.AddAsync(loc);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> ChangeStateDriver(string driverId, bool state)
        {
            try
            {               
                Driver? driv = await DB.Drivers.Include(a=>a.IdentityUser).SingleOrDefaultAsync(a=>a.IdentityUser!.PhoneNumber==driverId);
                if (driv == null) { return false; }
                driv.IsEmpty = state;
                DB.Drivers.Update(driv);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> DeleteDriver(Guid id)
        {
            try
            {
                Driver? driver = await DB.Drivers.Include(a=>a.IdentityUser).SingleOrDefaultAsync(a=>a.Id == id);
                if (driver == null) { return false; }                
                DB.Drivers.Remove(driver);
                await UserManager.DeleteAsync(driver.IdentityUser!);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> DeleteTermsOfUseAndPrivacy(Guid id)
        {
            try
            {
                TermsOfUseAndPrivacy? term = await DB.TermsOfUseAndPrivacy.FindAsync(id);
                if (term == null) { return false; }
                DB.TermsOfUseAndPrivacy.Remove(term);
                await DB.SaveChangesAsync();
                return true;

            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> DeleteTrip(string userId, Guid id)
        {
            Trip? trip = await DB.Trips.Include(a => a.User).SingleOrDefaultAsync(a => a.Id == id);
            if (trip == null) { return false; }
            User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
            if (user != trip.User) { return false; }
            if (trip.Accepted != null)
            {
                trip.User!.Rating += 1;
                DB.UserInfo.Update(trip.User);
            }
            DB.Trips.Remove(trip);
            await DB.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult<bool>> DeleteUserLocation(Guid locationId)
        {
            try
            {
                Location? loc = await DB.Locations.FindAsync(locationId);
                if (loc == null) { return false; }
                DB.Locations.Remove(loc);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> EndedTrip(Guid id)
        {
            try
            {
                Trip? trip = await DB.Trips.Include(a=>a.Drive!.IdentityUser).SingleOrDefaultAsync(a=>a.Id == id);
                if (trip == null) { return false; }
                trip.Ended = DateTime.Now;
                trip.Drive!.IsEmpty = true;
                DB.Trips.Update(trip);
                DB.Drivers.Update(trip.Drive);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<List<TripForWebSocketVW>>> GetAllTripForDriver(string userId, double lat, double lon)
        {
            var trAcc = await DB.Trips.Include(a => a.User!.UserIdentity).Include(a=>a.Drive!.IdentityUser)
                .Where(a => a.Drive!.IdentityUser!.PhoneNumber==userId && a.Ended == null).ToListAsync();
            var tr = new List<Trip>();
            var list = new List<TripForWebSocketVW>();
            var loc = new GoogleLocationService(apikey: "AIzaSyCxsin6TH7ouxNCDVoRp7IJihc4JxThkG8");
            if (trAcc.Count > 0) {tr = trAcc;}
            else {tr = await DB.Trips.Include(a => a.User!.UserIdentity).Where(a => a.Accepted == null).ToListAsync();
            }
            foreach (var t in tr) {
                var m = Math.Sqrt(Math.Pow((double)(lat - t!.FromLate!), 2) + Math.Pow((double)(lon! - t.FromLong!), 2)) * 100;
                if (m <= 10)
                {
                    var tri = new TripForWebSocketVW
                    {
                        Id = t!.Id,
                        FromLate = t.FromLate,
                        FromLong = t.FromLong,
                        Price = t.Price,
                        ToLate = t.ToLate,
                        ToLong = t.ToLong,
                        Distance = m,
                        Start = loc.GetAddressFromLatLang((double)t.FromLate!, (double)t.FromLong!).Address,
                        End = loc.GetAddressFromLatLang((double)t.ToLate!, (double)t.ToLong!).Address,
                        IsAccepted = t.Accepted != null,
                    };
                    list.Add(tri);
                }
            }
            return list;
            
        }
       
        public async Task<ActionResult<List<ShowTripVM>>> GetAllTrip(string phone)
        {
            try
            {
                List<Trip> trips = await DB.Trips.Include(a => a.User!.UserIdentity).Include(d=>d.Drive!.IdentityUser).Where(a => a.User!.UserIdentity!.PhoneNumber == phone).ToListAsync();
                List<ShowTripVM> showTripVMs = new List<ShowTripVM>();                
                foreach (Trip trip in trips)
                {
                        ShowTripVM showTrip = new ShowTripVM
                        {
                            Accepted = trip.Accepted,
                            CarColor = trip.Drive == null ? null : trip.Drive.CarColor,
                            CarNumber = trip.Drive == null ? null : trip.Drive.CarNumber,
                            CarType = trip.Drive == null ? null : trip.Drive.CarType,
                            Created = trip.Created,
                            FirtName = trip.Drive == null ? null : trip.Drive!.FirtName,
                            FromLate = trip.FromLate,
                            FromLong = trip.FromLong,
                            Id = trip.Id,
                            LastName = trip.Drive == null ? null : trip.Drive.LastName,
                            Phone = trip.User!.UserIdentity!.PhoneNumber,
                            Price = trip.Price,
                            Ended = trip.Ended,
                            ToLate = trip.ToLate,
                            ToLong = trip.ToLong,
                            UserName = trip.User.Name,
                            UserRating = trip.User.Rating,
                        };
                        showTripVMs.Add(showTrip);
                    
                }
                return showTripVMs;
            }
            catch { throw; }
        }

        public async Task<ActionResult<DriverVM>?> GetDriver(Guid id)
        {
            try
            {
                Driver? driver = await DB.Drivers.Include(a => a.IdentityUser).SingleOrDefaultAsync(a => a.Id == id);
                if (driver == null) { return null; }
                DriverVM driverVM = new DriverVM
                {
                    Balance = driver.Balance,
                    CarColor = driver.CarColor,
                    CarNumber = driver.CarNumber,
                    CarType = driver.CarType,
                    FatherName = driver.FatherName,
                    FirtName = driver.FirtName,
                    LastName = driver.LastName,
                    LicenType = driver.LicenType,
                    Phone = driver.IdentityUser!.PhoneNumber,
                };
                return driverVM;
            }
            catch { throw; }
        }

        public async Task<ActionResult<List<TermsOfUseAndPrivacy>>> GetTermsOfUseAndPrivacy(bool isPrivacy)
        {
            try
            {
                List<TermsOfUseAndPrivacy> terms = await DB.TermsOfUseAndPrivacy.Where(a => a.isPrivacy == isPrivacy).ToListAsync();
                return terms;
            }
            catch { throw; }
        }

        public async Task<ActionResult<ShowTripVM>?> GetTrip(Guid id)
        {
            try
            {
                Trip? trip = await DB.Trips.Include(a=>a.Drive!.IdentityUser).Include(b=>b.User!.UserIdentity).SingleOrDefaultAsync(a=>a.Id == id);
                if (trip == null) { return null; }
                ShowTripVM showTrip = new ShowTripVM
                {
                    Accepted = trip.Accepted,
                    CarColor = trip.Drive == null ? null : trip.Drive.CarColor,
                    CarNumber = trip.Drive == null ? null : trip.Drive.CarNumber,
                    CarType = trip.Drive == null ? null : trip.Drive.CarType,
                    Created = trip.Created,
                    FirtName = trip.Drive == null ? null : trip.Drive!.FirtName,
                    FromLate = trip.FromLate,
                    FromLong = trip.FromLong,
                    Id = trip.Id,
                    LastName = trip.Drive == null ? null : trip.Drive.LastName,
                    Phone = trip.User!.UserIdentity!.PhoneNumber,
                    Price = trip.Price,
                    Ended = trip.Ended,
                    ToLate = trip.ToLate,
                    ToLong = trip.ToLong,
                    UserName = trip.User.Name,
                    UserRating = trip.User.Rating,
                };
                return showTrip;
            }
            catch { throw; }
        }

        public async Task<ActionResult<RegisterVM>?> UpdateUserInfo(string userId,RegisterVM info)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
                if (user == null) { return null; }
                user.Name = info.Name;
                user.UserIdentity!.PhoneNumber = info.Phone;
                user.UserIdentity.Email = info.Email;
                DB.UserInfo.Update(user);
                await DB.SaveChangesAsync();
                return new RegisterVM { 
                    Email = user.UserIdentity.Email,
                    Phone = user.UserIdentity.PhoneNumber,
                    Name = user.Name,
                };
            }
            catch { throw; }
        }

        public async Task<ActionResult<List<ShowUserLocationVM>>> GetUserLocations(string userId)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
                List<Location> locations = await DB.Locations.Include(a => a.User!.UserIdentity).Where(a => a.User!.UserIdentity!.PhoneNumber == userId).ToListAsync();
                List<ShowUserLocationVM> showUserLocationVMs = new List<ShowUserLocationVM>();
                foreach (Location loc in locations)
                {
                    showUserLocationVMs.Add(new ShowUserLocationVM
                    {
                        Id = loc.Id,
                        Late = loc.Late,
                        Long = loc.Long,
                        Name = loc.Name,
                    });
                }
                return showUserLocationVMs;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> UpdateDriver(Guid id, DriverVM driver)
        {
            try
            {
                Driver? driv = await DB.Drivers.FindAsync(id);
                if (driv == null) { return false; }
                driv.CarNumber = driver.CarNumber;
                driv.FatherName = driver.FatherName;
                driv.FirtName = driver.FirtName;
                driv.CarType = driver.CarType;
                driv.Balance = driver.Balance;
                driv.CarColor = driver.CarColor;
                driv.LastName = driver.LastName;
                driv.LicenType = driver.LicenType;
                DB.Update(driv);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<bool>> UpdateTermsOfUseAndPrivacy(Guid id, PrivacyAndTermVM privacy)
        {
            try
            {
                TermsOfUseAndPrivacy? term = await DB.TermsOfUseAndPrivacy.FindAsync(id);
                if (term == null) { return false; }
                term.Title = privacy.Title;
                term.Text = privacy.Text;
                term.isPrivacy = privacy.IsPrivacy;
                DB.TermsOfUseAndPrivacy.Update(term);
                await DB.SaveChangesAsync();
                return true;
            }
            catch { throw; }
        }

        public async Task<ActionResult<RegisterVM>?> GetUserInfo(string userId)
        {
            try
            {
                User? user = await DB.UserInfo.Include(a => a.UserIdentity).SingleOrDefaultAsync(a => a.UserIdentity!.PhoneNumber == userId);
                if (user == null) { return null; }
                return new RegisterVM
                {
                    Email = user.UserIdentity!.Email,
                    Name = user.Name,
                    Phone = user.UserIdentity.PhoneNumber
                };
            }
            catch { throw; }
        }

        public async Task<ActionResult<List<TripForUser>>> GetAllTripForUser(string userId)
        {
            try
            {
                var loc = new GoogleLocationService(apikey: "AIzaSyCxsin6TH7ouxNCDVoRp7IJihc4JxThkG8");
                var list = await DB.Trips
                    .Include(u=>u.User!.UserIdentity)
                    .Include(d=>d.Drive!.IdentityUser)
                    .Where(a=>a.User!.UserIdentity!.PhoneNumber == userId).ToListAsync();
                var listVM = new List<TripForUser>();
                foreach (var trip in list)
                {
                    var tr = new TripForUser { 
                        Accepted = trip.Accepted,
                        CarColor = trip.Drive!.CarColor,
                        CarNumber = trip.Drive.CarNumber,
                        CarType = trip.Drive.CarType,
                        Created = trip.Created,
                        Distance = Math.Sqrt(Math.Pow((double)(trip.ToLate! - trip.FromLate!), 2) + Math.Pow((double)(trip.ToLong! - trip.FromLong!), 2)) * 100,
                        DriverName = $"{trip.Drive.FirtName} {trip.Drive.LastName}",                        
                        Ended = trip.Ended,
                        Id = trip.Id,
                        Price = trip.Price,
                        Start = loc.GetAddressFromLatLang((double)trip.FromLate!, (double)trip.FromLong!).Address,
                        End = loc.GetAddressFromLatLang((double)trip.ToLate!, (double)trip.ToLong!).Address,
                    };
                }
                return listVM;
            }
            catch { throw; }
        }
    }
}