using jwt;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taxi_api.Models;

namespace taxi_api.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ConnectWithUs> ConnectWithUs { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<TermsOfUseAndPrivacy> TermsOfUseAndPrivacy { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<VirefyCode> VirefyCodes { get; set;}
        public DbSet<User> UserInfo { get; set; }
        public DbSet<StaticValue> StaticValues { get; set; }
    }
}
