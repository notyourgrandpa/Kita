using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kita.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Kita.Server.Data
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<JwtSecurityToken> GetTokenAsync(ApplicationUser user)
        {
            var jwt = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: await GetClaimsAsync(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"])),
                signingCredentials: GetSigningCredentials());
            return jwt;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(
                           _configuration["JwtSettings:SecurityKey"]!);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret,
                           SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(
                 ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var rolePermissions = new Dictionary<string, Dictionary<string, string[]>>
            {
                ["Administrator"] = new Dictionary<string, string[]>
                {
                    ["inventory-log"] = new[] { "edit", "view", "delete" },
                    ["product"] = new[] { "edit", "view" },
                    ["user"] = new[] { "edit", "view", "delete" }
                },
                ["Manager"] = new Dictionary<string, string[]>
                {
                    ["inventory-log"] = new[] { "view" },
                    ["product"] = new[] { "view" }
                },
                ["Staff"] = new Dictionary<string, string[]>
                {
                    ["inventory-log"] = new[] { "view" }
                },
                ["RegisteredUser"] = new Dictionary<string, string[]>
                {
                    ["inventory-log"] = new[] { "view" }
                }
            };

            //Merge permissions from all roles the user has
            var userPermissions = new Dictionary<string, string[]>();
            foreach (var role in roles)
            {
                if (rolePermissions.ContainsKey(role))
                {
                    foreach (var kvp in rolePermissions[role])
                    {
                        if (!userPermissions.ContainsKey(kvp.Key))
                            userPermissions[kvp.Key] = kvp.Value;
                        else
                            userPermissions[kvp.Key] = userPermissions[kvp.Key].Union(kvp.Value).ToArray();
                    }
                }
            }

            var permissionsJson = System.Text.Json.JsonSerializer.Serialize(userPermissions);
            claims.Add(new Claim("permissions", permissionsJson));

            return claims;
        }
    }
}
