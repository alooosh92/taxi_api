using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using taxi_api.Data;
using taxi_api.Models;
using taxi_api.VModels;

namespace taxi_api.Controllers
{
    public class HubController:Hub
    {
        public HubController(UserManager<IdentityUser> userManager,ApplicationDbContext dB)
        {
            UserManager = userManager;
            DB = dB;
        }

        public UserManager<IdentityUser> UserManager { get; }
        public ApplicationDbContext DB { get; }
        public async void SendDriver(string phone,double lat,double lon)
        {
            var d =  DB.Drivers.Include(a => a.IdentityUser).SingleOrDefault(a => a.IdentityUser!.PhoneNumber == phone);
           if(d != null)
            {
                ShowDriverInWebSocketVM dr = new ShowDriverInWebSocketVM
                {
                    CarColor = d!.CarColor,
                    CarNumber = d.CarNumber,
                    CarType = d.CarType,
                    Name = $"{d.FirtName} {d.LastName}",
                    Phone = phone,
                    IsEmpty = d.IsEmpty,
                    PhonetripAccepted = null,
                };
                if (d.IsEmpty == false)
                {
                    var t = await DB.Trips.
                        Include(a => a.User!.UserIdentity).
                        Include(d => d.Drive!.IdentityUser).
                        SingleOrDefaultAsync(a => a.Drive!.IdentityUser!.PhoneNumber == phone && a.Accepted != null && a.Ended == null);
                    if (t != null)
                    {
                        dr.PhonetripAccepted = t!.User!.UserIdentity!.PhoneNumber;
                    }
                }
                await Clients.Others.SendAsync("SendDriver", "car", dr, lat, lon);
            }
        }
        public async void SendTrip(string phone)
        {
            var t = DB.Trips.Include(a => a.User!.UserIdentity).SingleOrDefault(a => a.User!.UserIdentity!.PhoneNumber == phone && a.Accepted == null);
            if (t != null)
            {
                var loc = new GoogleLocationService(apikey: "AIzaSyCxsin6TH7ouxNCDVoRp7IJihc4JxThkG8");
                var tri = new TripForWebSocketVW
                {
                    Id = t!.Id,
                    FromLate = t.FromLate,
                    FromLong = t.FromLong,
                    Price = t.Price,
                    ToLate = t.ToLate,
                    ToLong = t.ToLong,
                    Start = loc.GetAddressFromLatLang((double)t.FromLate!, (double)t.FromLong!).Address,
                    End = loc.GetAddressFromLatLang((double)t.ToLate!, (double)t.ToLong!).Address,
                };
                await Clients.Others.SendAsync("SendTrip", "trip", tri);
            }
        }
        public async void AcceptTrip(Guid id)
        {
            await Clients.Others.SendAsync("AcceptTrip", "AcceptTrip", id);
        }
    }
}
