using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;

namespace ApexVoxApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly ITenantContext _unitOfWork;

        public UserService(IOptions<AppSettings> appSettings, ITenantContext unitOfWork)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public User Authenticate(string username, string password)
        {
            var user = _unitOfWork.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("TenantId", user.TenantId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        public User Create(string tenantName)
        {
            var tenant = _unitOfWork.Tenants.SingleOrDefault(x=>x.Name == tenantName);

            var user = new User()
            {
                FirstName = "John_"+tenantName,
                LastName = "Doe",
                Username = "admin_" + tenantName,
                Password = "password",
                TenantId = tenant.Id,
                Role = Role.Admin
            };

            _unitOfWork.Users.Add(user);

            _unitOfWork.SaveChanges();

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _unitOfWork.Users;
        }

        public User GetById(long id)
        {
            var user = _unitOfWork.Users.SingleOrDefault(x => x.Id == id);

            return user;
        }
    }
}