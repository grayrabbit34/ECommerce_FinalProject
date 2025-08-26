using System;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.WebApi.Jwt;

namespace ECommerce.WebApi.Jwt
{
    
    /// JWT üretiminden sorumlu yardımcı sınıf.

    public static class JwtHelper
    {
        
        /// Verilen bilgilere göre imzalı bir JWT token üretir.
        /// Dönen tuple: (token string'i, token bitiş zamanı - UTC)
       
        public static string GenerateJwtToken(JwtDto dto)
        {
            // Simetrik anahtar (SecretKey) -> HMAC SHA256 ile imzalanır
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(dto.SecretKey));
            var crediantials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            // Token'a eklenecek claim'ler (kim? yetkisi ne?)
            var claims = new List<Claim>
            {
                new(JwtClaimNames.Id, dto.Id.ToString()),
                new(JwtClaimNames.Email, dto.Email),
                new(JwtClaimNames.FirstName, dto.FirstName),
                new(JwtClaimNames.LastName, dto.LastName),
                new(JwtClaimNames.Role, dto.Role.ToString()),

                // ASP.NET Core [Authorize(Roles="...")] için standart rol claim'i

                new(ClaimTypes.Role, dto.Role.ToString()),

                // NameIdentifier çoğu kütüphanede "kullanıcı id" gibi kullanılır

                new(ClaimTypes.NameIdentifier, dto.Id.ToString())
            };


        

            // Süre sonu (UTC) – saat farkı problemlerini azaltır
            var expires = DateTime.UtcNow.AddMinutes(dto.ExpireMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: dto.Issuer,
                audience: dto.Audience,
                claims: claims,
                null,
                expires: expires,
                signingCredentials: crediantials
            );

            // Token string'e çevrilip döndürülür
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }
    }

}


