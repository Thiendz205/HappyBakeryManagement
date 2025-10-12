using HappyBakeryManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // 👉 Đăng ký cả 2 DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDbContext<HappyBakeryContext>(options =>
                options.UseSqlServer(connectionString));
            //builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // cấu hình password/lockout nếu    // Cấu hình password / lockout / confirm nếu cần
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            // 👉 GỌI SEED Ở ĐÂY
            SeedDataAsync(app).Wait();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
            async Task SeedDataAsync(WebApplication app)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    // 🔹 1. Tạo roles nếu chưa có
                    string[] roles = new[] { "Admin", "User" };
                    foreach (var r in roles)
                    {
                        if (!await roleMgr.RoleExistsAsync(r))
                            await roleMgr.CreateAsync(new IdentityRole(r));
                    }

                    // 🔹 2. Tạo tài khoản Admin mặc định
                    var adminEmail = "admin@local.test";
                    var admin = await userMgr.FindByEmailAsync(adminEmail);
                    if (admin == null)
                    {
                        admin = new IdentityUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true
                        };
                        await userMgr.CreateAsync(admin, "Admin@123");
                        await userMgr.AddToRoleAsync(admin, "Admin");
                    }

                    // 🔹 3. Tạo tài khoản User mặc định
                    var userEmail = "user@local.test";
                    var user = await userMgr.FindByEmailAsync(userEmail);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = userEmail,
                            Email = userEmail,
                            EmailConfirmed = true
                        };
                        await userMgr.CreateAsync(user, "User@123");
                        await userMgr.AddToRoleAsync(user, "User");
                    }
                }
            }
        }
    }
}
