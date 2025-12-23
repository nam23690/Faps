using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FAP.Test.Integration.Helpers;

public static class JwtTokenHelper
{
    public static string GenerateTestJwt(string CampusCode, string Permision)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("798jkijuhwd98uy679hkjjkjlkjldshjlhj"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var permissionList = Permision?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim(JwtRegisteredClaimNames.Email, "testuser@fpt.edu.vn"),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim("campus", CampusCode),

        };
        claims.AddRange(permissionList.Select(p => new Claim("Permission", p)));
        var token = new JwtSecurityToken(
            issuer: "FAP.API.DATAO",
            audience: "University.Users",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

