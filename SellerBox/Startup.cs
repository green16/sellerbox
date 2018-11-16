﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Common;
using SellerBox.Common.Schedulers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System.Security.Claims;

namespace SellerBox
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton(Configuration);

            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")), ServiceLifetime.Transient);

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

            services.AddAuthentication()
            .AddVkontakte(options =>
            {
                options.ClientId = Configuration.GetValue<string>("VkApplicationId");
                options.ClientSecret = Configuration.GetValue<string>("VkApplicationPassword");
                options.SaveTokens = true;

                // Request for permissions https://vk.com/dev/permissions?f=1.%20Access%20Permissions%20for%20User%20Token
                options.Scope.Add("photos,wall,offline,groups,audio,docs,video");

                // In this case email will return in OAuthTokenResponse, 
                // but all scope values will be merged with user response
                // so we can claim it as field
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            }, "ВКонтакте");

            services.AddMvc();

            services.AddSignalR();

            services.AddDistributedMemoryCache();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddTransient<UserHelperService>();
            services.AddTransient<VkPoolService>();

            services.AddHostedService<VkCallbackScheduler>();
            services.AddHostedService<TextScenariosScheduler>();
            services.AddHostedService<MessagingScheduler>();
            services.AddHostedService<BirthdayScenariosScheduler>();
            services.AddHostedService<RepostScheduler>();
            services.AddHostedService<ShortUrlsScheduler>();
            services.AddHostedService<NotifyScheduler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                env.UseRootNodeModules();
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Common.Hubs.SubscriberSyncHub>("/subscribersynchub");
                routes.MapHub<Common.Hubs.MessagingHub>("/messaginghub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                    name: "shortLinksWithSubscriber",
                    template: Common.Helpers.UrlShortenerHelper.RoutePrefix + "={key}&{id}",
                    defaults: new { controller = "Home", action = "ShortLinkWithSubscriber" });
                routes.MapRoute(
                    name: "shortLinksWithoutSubscriber",
                    template: Common.Helpers.UrlShortenerHelper.RoutePrefix + "={key}",
                    defaults: new { controller = "Home", action = "ShortLinkWithoutSubscriber" });
            });
        }
    }
}
