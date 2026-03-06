using CommunityToolkit.Maui;
using ei8.Avatar.Installer.Application;
using ei8.Avatar.Installer.Application.Avatar;
using ei8.Avatar.Installer.Application.IdentityAccess;
using ei8.Avatar.Installer.Domain.Model;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.Domain.Model.Configuration;
using ei8.Avatar.Installer.Domain.Model.IdentityAccess;
using ei8.Avatar.Installer.Domain.Model.Mapping;
using ei8.Avatar.Installer.Domain.Model.Template;
using ei8.Avatar.Installer.IO.Process.Services.Avatars;
using ei8.Avatar.Installer.IO.Process.Services.IdentityAccess;
using ei8.Avatar.Installer.IO.Process.Services.Template;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.ViewModels;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.ViewModels.AvatarSettings;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.Views;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.Views.AvatarSettings;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;

namespace ei8.Avatar.Installer.Port.Adapter.UI.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();

        builder.Services.AddLogging(
            configure =>
            {
                configure.AddDebug();
            }
        );
#endif

        builder.Logging.AddInMemoryLogger(
            options =>
            {
                options.MaxLines = 1024;
                options.MinLevel = LogLevel.Debug;
                options.MaxLevel = LogLevel.Critical;
            });

        #region Pages
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<CreateAvatarPage>();

        builder.Services.AddSingleton<NeuronPermitsPage>();
        builder.Services.AddTransient<NeuronPermitDetailsPage>();

        builder.Services.AddSingleton<RegionPermitsPage>();
        builder.Services.AddTransient<RegionPermitDetailsPage>();

        builder.Services.AddSingleton<UsersPage>();
        builder.Services.AddTransient<UserDetailsPage>();

        builder.Services.AddSingleton<AvatarApiSettingsPage>();
        builder.Services.AddSingleton<EventSourcingSettingsPage>();
        builder.Services.AddSingleton<CortexGraphSettingsPage>();
        builder.Services.AddSingleton<CortexGraphPersistenceSettingsPage>();
        builder.Services.AddSingleton<AvatarApiSettingsPage>();
        builder.Services.AddSingleton<IdentityAccessSettingsPage>();
        builder.Services.AddSingleton<CortexLibrarySettingsPage>();
        builder.Services.AddSingleton<CortexDiaryNucleusSettingsPage>();
        builder.Services.AddSingleton<CortexChatNucleusSettingsPage>();
        #endregion

        #region ViewModels
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<CreateAvatarViewModel>();

        builder.Services.AddSingleton<NeuronPermitsViewModel>();
        builder.Services.AddTransient<NeuronPermitDetailsViewModel>();

        builder.Services.AddSingleton<RegionPermitsViewModel>();
        builder.Services.AddTransient<RegionPermitDetailsViewModel>();

        builder.Services.AddSingleton<UsersViewModel>();
        builder.Services.AddTransient<UserDetailsViewModel>();

        builder.Services.AddSingleton<EventSourcingSettingsViewModel>();
        builder.Services.AddSingleton<CortexGraphSettingsViewModel>();
        builder.Services.AddSingleton<CortexGraphPersistenceSettingsViewModel>();
        builder.Services.AddSingleton<AvatarApiSettingsViewModel>();
        builder.Services.AddSingleton<IdentityAccessSettingsViewModel>();
        builder.Services.AddSingleton<CortexLibrarySettingsViewModel>();
        builder.Services.AddSingleton<CortexDiaryNucleusSettingsViewModel>();
        builder.Services.AddSingleton<CortexChatNucleusSettingsViewModel>();
        #endregion

        builder.Services
                .AddScoped<ITemplateService, GithubTemplateService>()
                .AddScoped<IConfigurationRepository, JsonConfigurationRepository>()
                .AddScoped<IAvatarItemWriteRepository, AvatarItemWriteRepository>()
                .AddScoped<IAvatarItemReadRepository, AvatarItemReadRepository>()
                .AddScoped<IAvatarMapperService, AvatarMapperService>()
                .AddScoped<IAvatarApplicationService, AvatarApplicationService>();
        builder.Services.AddAutoMapper(cfg => { }, typeof(AvatarAutoMapperProfile));

        builder.Services.AddSingleton<IAvatarContextService, AvatarContextService>();
        builder.Services.AddSingleton<IProgressService, ProgressService>();

        builder.Services.AddSingleton<INeuronPermitRepository, NeuronPermitRepository>();
        builder.Services.AddSingleton<IRegionPermitRepository, RegionPermitRepository>();
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IAvatarSettingsRepository, AvatarSettingsRepository>();

        builder.Services.AddSingleton<INeuronPermitApplicationService, NeuronPermitApplicationService>();
        builder.Services.AddSingleton<IRegionPermitApplicationService, RegionPermitApplicationService>();
        builder.Services.AddSingleton<IUserApplicationService, UserApplicationService>();
        builder.Services.AddSingleton<IAvatarSettingsApplicationService, AvatarSettingsApplicationService>();

        return builder.Build();
    }
}
