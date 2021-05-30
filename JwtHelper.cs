using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FastWXCallBackend
{
    public class JwtHelper
    {
        IConfiguration Configuration { get; }

        public JwtHelper(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public string CreateToken(string userId)
        {
            IConfigurationSection jwtSection = Configuration.GetSection("JWT");
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.Name, userId) };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("SecurityKey")));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                                            issuer: jwtSection.GetValue<string>("Issuer"), 
                                            audience: jwtSection.GetValue<string>("Audience"),
                                            claims: claims,
                                            expires: DateTime.Now.AddDays(jwtSection.GetValue<int>("ExpiresDays")),
                                            signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
