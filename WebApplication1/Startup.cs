using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Schedulers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private void SetLogger(string dbConnectionString)
        {
            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            loggingConfiguration.Variables["botname"] = "admin";
            var dbTarget = new DatabaseTarget
            {
                ConnectionString = dbConnectionString,
                CommandText = @"INSERT INTO dbo.Logs ([Logged], [Level], [Message], [Logger], [CallSite], [Exception]) VALUES (@Logged, @Level, @Message, @Logger, @Callsite, @Exception)"
            };

            dbTarget.Parameters.Add(new DatabaseParameterInfo("@Logged", new SimpleLayout("${date}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@Level", new SimpleLayout("${level:format=Ordinal}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@Message", new SimpleLayout("${message}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@Logger", new SimpleLayout("${logger}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@CallSite", new SimpleLayout("${callsite}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@Exception", new SimpleLayout("${exception:tostring}")));

            loggingConfiguration.AddTarget("database", dbTarget);

            LoggingRule dbRule = new LoggingRule("*", LogLevel.Trace, dbTarget);

            loggingConfiguration.LoggingRules.Add(dbRule);

            LogManager.Configuration = loggingConfiguration;
        }

        private static async Task RedirectToAuthorizationEndpoint(RedirectContext<OAuthOptions> context)
        {
            // Ensure the redirect_uri is https
            System.Diagnostics.Trace.WriteLine(context.RedirectUri);
            context.Response.Redirect(context.RedirectUri);
            await Task.FromResult(0);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            string dbConnectionString = Configuration.GetConnectionString("dbConnectionStringDebug");
#else
            string dbConnectionString = Configuration.GetConnectionString("dbConnectionStringRelease");
#endif

            services.AddSingleton(Configuration);
            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DatabaseContext>(options => options.UseSqlServer(dbConnectionString), ServiceLifetime.Transient);

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

            services.AddAuthentication()
            .AddVkontakte(options =>
            {
                options.ClientId = Logins.VkApplicationId.ToString();
                options.ClientSecret = Logins.VkApplicationPassword;
                options.SaveTokens = true;

                // Request for permissions https://vk.com/dev/permissions?f=1.%20Access%20Permissions%20for%20User%20Token
                options.Scope.Add("photos,wall,offline,groups,audio,docs,video");

                options.Events = new OAuthEvents
                {
                    OnRedirectToAuthorizationEndpoint = async context => { await RedirectToAuthorizationEndpoint(context); }
                };

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
            SetLogger(dbConnectionString);

            services.AddTransient<UserHelperService>();
            services.AddTransient<VkPoolService>();

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, VkCallbackWorkerService>();

            services.AddHostedService<BirthdayScenariosScheduler>();
            services.AddHostedService<RepostScheduler>();
            services.AddHostedService<TextScenariosScheduler>();

            services.AddHostedService<MessagingScheduler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
