using ei8.Avatar.Installer.Application.Avatar;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.Domain.Model.Configuration;
using ei8.Avatar.Installer.Domain.Model.Mapping;
using ei8.Avatar.Installer.Domain.Model.Template;
using ei8.Avatar.Installer.IO.Process.Services.Avatars;
using ei8.Avatar.Installer.IO.Process.Services.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ei8.Avatar.Installer.CLI
{
    internal class Program
    {
        // commandline args
        // --config = config path (JSON file, DB, etc, can be relative or absolute)
        static async Task Main(string[] args)
        {
            try
            {
                var builder = Host.CreateApplicationBuilder(args);

                builder.Services
                                .AddScoped<ITemplateService, GithubTemplateService>()
                                .AddScoped<IConfigurationRepository, JsonConfigurationRepository>()
                                .AddScoped<IAvatarItemWriteRepository, AvatarItemWriteRepository>()
                                .AddScoped<IAvatarMapperService, AvatarMapperService>()
                                .AddScoped<IAvatarApplicationService, AvatarApplicationService>();

                builder.Services.AddAutoMapper(cfg => { }, typeof(AvatarAutoMapperProfile));

                builder.Logging.AddConsole();

                using (IHost host = builder.Build())
                {
                    var avatarApplicationService = host.Services.GetRequiredService<IAvatarApplicationService>();

                    var configPath = builder.Configuration.GetValue<string>("config");

                    var configObject = await avatarApplicationService.ReadAvatarConfiguration(configPath);

                    await avatarApplicationService.CreateAvatarAsync(configObject);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Complete. Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}