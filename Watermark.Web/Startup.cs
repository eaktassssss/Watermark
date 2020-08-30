using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Watermark.AzureStorage.BlobStorage.Repository.Abstract;
using Watermark.AzureStorage.BlobStorage.Repository.Concrete;
using Watermark.AzureStorage.ClientConnetion;
using Watermark.AzureStorage.QueueStorage.Abstract;
using Watermark.AzureStorage.QueueStorage.Concrete;
using Watermark.AzureStorage.TableStorage.Repository.Abstract;
using Watermark.AzureStorage.TableStorage.Repository.Concrete;
using Watermark.Common.Hubs;

namespace Watermark.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            StorageConnection.ConnectionString = Configuration.GetSection("AzureStorageConnection")["ASConnectionStringName"];

            services.AddScoped(typeof(ITableStorageRepository<>), typeof(TableStorageBaseRepository<>));
            services.AddScoped<IBlobStorageRepository, BlobStorageBaseRepository>();
            services.AddScoped<IQueueStorageService, QueueStorageService>();
           
            services.AddSignalR();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/NotificationHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Picture}/{action=Index}/{id?}");
            });
        }
    }
}
