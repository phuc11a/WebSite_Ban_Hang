﻿using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T10202599.Web
{
    /// <summary>
    /// Những thông tin cần dùng để biểu diễn "danh tính" của người dùng
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Displayname { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Tạo ra chứng nhận để ghi nhận danh tính của người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            //tạo danh sách các Claim, mỗi Claim lưu giữ 1 thông tin trong danh tính của người dùng
            List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(Displayname), Displayname),
                new Claim(nameof(Photo), Photo),
            };
            if (Roles != null)
                foreach (var role in Roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

            //Tạo Identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Tạo Principal ( Giấy chứng nhận)
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
