using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using taxi_api.Data;
using taxi_api.VModels;
using taxi_api.Models;

namespace jwt
{
    //Controller
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthServies _authServies;
        public AuthenticationController(IAuthServies authServies)
        {
            _authServies = authServies;
            
        }
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<bool>> Register([FromBody] UserModel userModel)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);
            return await _authServies.Register(userModel);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<bool>> Login(string phone)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return await _authServies.Login(phone);
        }
        [HttpPost]
        [Route("VerifyPhone")]
        public async Task<ActionResult<AuthModel>> VerifyPhone([FromBody] VirefyVM virefy)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return await _authServies.VerifyPhone(virefy.Phone!,virefy.Code);
        }
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult<AuthModel>> RefreshToken([FromBody] string refToken)
        {
            return await _authServies.RefreshToken(refToken);
        }
        [HttpGet]
        [Route("CheckToken")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public bool CheckToken()
        {
            return true;
        }
    }
    //Models
    public class AuthModel
    {
        public string? Phone { get; set; }
        public string? Message { get; set; }
        public bool IsAuthanticated { get; set; }
        public string? Email { get; set; }
        public IList<string>? Roles { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireson { get; set; }
    }
    public class UserModel
    {
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class RefreshToken
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required]
        public DateTime? Expirson { get; set; }
        [Required]
        public DateTime? CreatedOn { get; set; }
        [AllowNull]
        public DateTime? RevokedON { get; set; }
    }
    public class JWTValues
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public double DurationInDays { get; set; }
    }
    //Interface
    public interface IAuthServies
    {
        Task<ActionResult<bool>> Register(UserModel userModel);
        Task<ActionResult<bool>> Login(string phone);
        Task<ActionResult<AuthModel>> RefreshToken(string token);
        Task<ActionResult<AuthModel>> VerifyPhone(string phone, int code);
    }
    //EmailSender
    public class EmailSender : IEmailSender
    {
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;
        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
    //AuthServies
    public class AuthServies : IAuthServies
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<JWTValues> _jwt;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        public AuthServies(UserManager<IdentityUser> userManager, IOptions<JWTValues> jwt, IEmailSender emailSender, ApplicationDbContext db)
        {
            _userManager = userManager;
            _jwt = jwt;
            _emailSender = emailSender;
            _db = db;
        }
        private async Task<JwtSecurityToken> CreateJwtSecurityToken(IdentityUser identityUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(identityUser);
            var roles = await _userManager.GetRolesAsync(identityUser);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,identityUser.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("uid",identityUser.Id)
            }.Union(userClaims).Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key!));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Value.Issuer,
                audience: _jwt.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.Value.DurationInDays).ToLocalTime(),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        private RefreshToken GeneraterRefreshToken(string userId)
        {
            var randomNumber = new byte[32];
            using var genertor = RandomNumberGenerator.Create();
            genertor.GetBytes(randomNumber);
            return new RefreshToken
            {
                UserId = userId,
                CreatedOn = DateTime.UtcNow,
                Expirson = DateTime.UtcNow.AddYears(1),
                Id = Guid.NewGuid().ToString(),
                Token = Convert.ToBase64String(randomNumber),
            };

        }
        public async Task<ActionResult<bool>> Register(UserModel userModel)
        {
            if (userModel == null || userModel.Phone == null) { return false; }
            var u = await _db.Users.AnyAsync(a => a.PhoneNumber == userModel.Phone);
            if (u) { return false; }
            var user = new IdentityUser
            {
                UserName = userModel.Phone!,
                PhoneNumber = userModel.Phone!,
                Email = userModel.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
            };
            var res = await _userManager.CreateAsync(user);
            if (!res.Succeeded) { return false; }
            await _userManager.AddToRoleAsync(user, "user");
            _db.VirefyCodes.Add(new VirefyCode() { Phone = userModel.Phone });
            User us = new User { Name = userModel.Name,UserIdentity = user };
            await _db.UserInfo.AddAsync(us);
            await _db.SaveChangesAsync();
            return true;
         }
        public async Task<ActionResult<bool>> Login(string phone)
        {
            if(phone == null) { return false; }
            var user = await _db.Users.Where(u => u.PhoneNumber == phone).SingleOrDefaultAsync();
            if (user == null) { return false; }
            var virefy = await _db.VirefyCodes.Where(v => v.Phone == phone).SingleOrDefaultAsync();
            if (virefy != null) {
                if (virefy.CreatedDate > DateTime.Now.AddHours(-1))
                { return true; }
            }
            _db.VirefyCodes.Add(new VirefyCode() { Phone = phone});
            await _db.SaveChangesAsync();
            return true;           
        }
        public async Task<ActionResult<AuthModel>> RefreshToken(string token)
        {
            var authModel = new AuthModel();
            var tok = await _db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == token);
            if (tok == null)
            {
                authModel.Message = "InValid token";
                return authModel;
            }
            if (!(tok.RevokedON == null && DateTime.UtcNow <= tok.Expirson))
            {
                authModel.Message = "Inactive token";
                return authModel;
            }
            tok.RevokedON = DateTime.UtcNow;
            var newRefreshToken = GeneraterRefreshToken(tok.UserId!);
            await _db.RefreshTokens!.AddAsync(newRefreshToken);
            _db.RefreshTokens.Update(tok);
            await _db.SaveChangesAsync();
            var user = await _userManager.FindByIdAsync(tok.UserId!);
            var jwtToken = await CreateJwtSecurityToken(user!);
            authModel.Message = "Every thing is ok";
            authModel.IsAuthanticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user!.Email;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpireson = newRefreshToken.Expirson;
            return authModel;
        }
        public async Task<bool> RevokeToken(string token)
        {
            var toke = await _db.RefreshTokens.SingleOrDefaultAsync(t => t.Token == token);
            if (toke == null) return false;
            if (!(toke.RevokedON == null && DateTime.UtcNow <= toke.Expirson)) return false;
            toke.RevokedON = DateTime.UtcNow;
            _db.RefreshTokens.Update(toke);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<ActionResult<AuthModel>> VerifyPhone(string phone, int code)
        {
            if (phone == null) { return new AuthModel { Message = "phone is null" }; }
            var virefy = _db.VirefyCodes.Where(a => a.Phone == phone).SingleOrDefault();
            if (virefy == null || virefy.Code != code || DateTime.Now.AddMinutes(-60) > virefy.CreatedDate)
            {
                if(virefy != null && DateTime.Now.AddMinutes(-60) > virefy.CreatedDate)
                {
                    _db.VirefyCodes.Remove(virefy);
                    await _db.SaveChangesAsync();
                }
                return new AuthModel { Message = "code is not match or expird" };
            }            
            var u = await _db.Users.Where(a => a.PhoneNumber == phone).SingleOrDefaultAsync();              
            var refreshToken = GeneraterRefreshToken(u!.Id);
            var rList = await _db.RefreshTokens.Where(a => a.UserId == u.Id).ToListAsync(); 
            foreach(var r in rList)
            {
                _db.RefreshTokens.Remove(r);
            }
            object value = await _db.RefreshTokens.AddAsync(refreshToken);
            var token = await CreateJwtSecurityToken(u);
            var back = new AuthModel
            {
                Message = "Every thing is ok",
                IsAuthanticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Roles = await _userManager.GetRolesAsync(u),
                Email = u.Email,
                Phone = u.PhoneNumber,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireson = refreshToken.Expirson
            };
            _db.VirefyCodes.Remove(virefy);
            await _db.SaveChangesAsync();
            return back;
        }
    }
    //Seed
    public class Seed
    {
        public static void Setting(WebApplicationBuilder builder)
        {
            builder.Services.Configure<JWTValues>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt => {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                //SingIn
                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
                opt.SignIn.RequireConfirmedAccount = false;
                //Password
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            builder.Services.AddTransient<IAuthServies, AuthServies>();
            builder.Services.AddTransient<IEmailSender, EmailSender>(a =>
                          new EmailSender(
                              builder.Configuration["EmailSender:Host"]!,
                              builder.Configuration.GetValue<int>("EmailSender:Port"),
                              builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                              builder.Configuration["EmailSender:UserName"]!,
                              builder.Configuration["EmailSender:Password"]!
                          )
                      );
        }
        public static async Task AddRoll(IServiceProvider provider, List<string> roles)
        {
            var scopFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var role = scopFactory.CreateScope();
            var ro = role.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (string roleName in roles)
            {
                if (!await ro.RoleExistsAsync(roleName))
                {
                    IdentityRole rol = new IdentityRole { Name = roleName, NormalizedName = roleName };
                    await ro.CreateAsync(rol);
                }
            }
        }
        public static async Task AddAdmin(IServiceProvider provider, string email)
        {
            var scopFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var user = scopFactory.CreateScope();
            var us = user.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (await us.FindByEmailAsync(email) == null)
            {
                IdentityUser use = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    PhoneNumber = email,
                };
                await us.CreateAsync(use);
                await us.AddToRoleAsync(use,"Admin");
            }

        }
    }
}


