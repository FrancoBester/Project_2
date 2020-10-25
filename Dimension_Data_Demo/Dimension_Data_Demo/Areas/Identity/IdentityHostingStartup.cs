using System;
using Dimension_Data_Demo.Areas.Identity.Data;
using Dimension_Data_Demo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DbContext = Dimension_Data_Demo.Data.DbContext;

[assembly: HostingStartup(typeof(Dimension_Data_Demo.Areas.Identity.IdentityHostingStartup))]
namespace Dimension_Data_Demo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<DbContext>();
            });
        }
    }
}