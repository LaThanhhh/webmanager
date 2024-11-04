using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Web.Infrastructure;
using Owin;
using System;

// Thêm using directive cho namespace chứa ApplicationUser
using QLBanHang.Models;
using Fluent.Infrastructure.FluentModel;

[assembly: OwinStartup(typeof(QLBanHang.Startup))]

namespace QLBanHang
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            InitUserRole(); // Gọi phương thức khởi tạo quyền người dùng
        }

        private void InitUserRole()
        {
            // Khởi tạo đối tượng DbContext
            ApplicationDbContext context = new ApplicationDbContext();

            // Khởi tạo UserManager và RoleManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Thêm logic khởi tạo quyền người dùng ở đây
            // Ví dụ: Tạo quyền "Admin"
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                //tạo user

            }

            // Tạo một người dùng và gán quyền "Admin" cho người dùng
            var user = new ApplicationUser();
            user.UserName = "admin@lott.com";
            user.Email = "admin@lott.com";


            var result = userManager.Create(user, "Password@123");

            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
