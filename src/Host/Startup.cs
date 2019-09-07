namespace Host
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nancy.Owin;

    public class Startup
    {
        readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            AppConfiguration appConfig = new AppConfiguration();
            config.Bind(appConfig);

            app.UseOwin(x =>
                x.UseNancy(opt =>
                    opt.Bootstrapper = new Bootstrapper()));
        }
    }
}
