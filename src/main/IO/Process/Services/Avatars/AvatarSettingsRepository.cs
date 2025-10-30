using ei8.Avatar.Installer.Common;
using ei8.Avatar.Installer.Domain.Model;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using neurUL.Common.Domain.Model;

namespace ei8.Avatar.Installer.IO.Process.Services.Avatars;

public class AvatarSettingsRepository : IAvatarSettingsRepository
{
    private readonly IAvatarContextService avatarContextService;

    public AvatarSettingsRepository(IAvatarContextService avatarContextService)
    {
        AssertionConcern.AssertArgumentNotNull(avatarContextService, nameof(avatarContextService));

        this.avatarContextService = avatarContextService;
    }

    public async Task<AvatarSettings> GetAsync()
    {
        var path = Path.Combine(this.avatarContextService.Avatar.Id, Constants.Filenames.VariablesEnv);
        var avatarSettings = new AvatarSettings();

        var envFile = await File.ReadAllLinesAsync(path);

        foreach (var line in envFile)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                continue;

            var index = line.IndexOf('=');

            // If no "=" found, go to the next line
            if (index == -1) continue;

            var key = line[..index].Trim();
            var value = line[(index + 1)..].Trim();

            switch (key.ToUpper())
            {
                #region EventSourcingSettings
                case Constants.EventSourcingSettingsEnv.DatabasePath:
                    avatarSettings.EventSourcing.DatabasePath = value;
                    break;
                case Constants.EventSourcingSettingsEnv.DisplayErrorTraces:
                    bool displayErrorTraces;
                    if (bool.TryParse(value, out displayErrorTraces))
                        avatarSettings.EventSourcing.DisplayErrorTraces = displayErrorTraces;
                    break;
                #endregion

                #region CortexGraphSettings
                case Constants.CortexGraphSettingsEnv.PollInterval:
                    int pollInterval;
                    if (int.TryParse(value, out pollInterval))
                        avatarSettings.CortexGraph.PollInterval = pollInterval;
                    break;
                case Constants.CortexGraphSettingsEnv.DbName:
                    avatarSettings.CortexGraph.DbName = value;
                    break;
                case Constants.CortexGraphSettingsEnv.DbUsername:
                    avatarSettings.CortexGraph.DbUsername = value;
                    break;
                case Constants.CortexGraphSettingsEnv.DbPassword:
                    avatarSettings.CortexGraph.DbPassword = value;
                    break;
                case Constants.CortexGraphSettingsEnv.DbUrl:
                    avatarSettings.CortexGraph.DbUrl = value;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultRelativeValues:
                    int defaultRelativeValues;
                    if (int.TryParse(value, out defaultRelativeValues))
                        avatarSettings.CortexGraph.DefaultRelativeValues = defaultRelativeValues;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultNeuronActiveValues:
                    int defaultNeuronActiveValues;
                    if (int.TryParse(value, out defaultNeuronActiveValues))
                        avatarSettings.CortexGraph.DefaultNeuronActiveValues = defaultNeuronActiveValues;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultTerminalActiveValues:
                    int defaultTerminalActiveValues;
                    if (int.TryParse(value, out defaultTerminalActiveValues))
                        avatarSettings.CortexGraph.DefaultTerminalActiveValues = defaultTerminalActiveValues;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultPageSize:
                    int defaultPageSize;
                    if (int.TryParse(value, out defaultPageSize))
                        avatarSettings.CortexGraph.DefaultPageSize = defaultPageSize;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultPage:
                    int defaultPage;
                    if (int.TryParse(value, out defaultPage))
                        avatarSettings.CortexGraph.DefaultPage = defaultPage;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultDepth:
                    if (int.TryParse(value, out int defaultDepth))
                        avatarSettings.CortexGraph.DefaultDepth = defaultDepth;
                    break;
                case Constants.CortexGraphSettingsEnv.DefaultDirectionValues:
                    if (int.TryParse(value, out int defaultDirectionValues))
                        avatarSettings.CortexGraph.DefaultDirectionValues = defaultDirectionValues;
                    break;
                #endregion

                #region AvatarApiSettings
                case Common.Constants.AvatarApiSettingsEnv.ResourceDatabasePath:
                    avatarSettings.AvatarApi.ResourceDatabasePath = value;
                    break;
                case Constants.AvatarApiSettingsEnv.RequireAuthentication:
                    bool requireAuthentication;
                    if (bool.TryParse(value, out requireAuthentication))
                        avatarSettings.AvatarApi.RequireAuthentication = requireAuthentication;
                    break;
                case Constants.AvatarApiSettingsEnv.AnonymousUserId:
                    avatarSettings.AvatarApi.AnonymousUserId = value;
                    break;
                case Constants.AvatarApiSettingsEnv.ProxyUserId:
                    Guid proxyUserId;
                    if (Guid.TryParse(value, out proxyUserId))
                        avatarSettings.AvatarApi.ProxyUserId = proxyUserId;
                    break;
                case Constants.AvatarApiSettingsEnv.TokenIssuerAddress:
                    avatarSettings.AvatarApi.TokenIssuerAddress = value;
                    break;
                case Constants.AvatarApiSettingsEnv.ApiName:
                    avatarSettings.AvatarApi.ApiName = value;
                    break;
                case Constants.AvatarApiSettingsEnv.ApiSecret:
                    avatarSettings.AvatarApi.ApiSecret = value;
                    break;
                case Constants.AvatarApiSettingsEnv.ValidateServerCertificate:
                    bool validateServerCertificate;
                    if (bool.TryParse(value, out validateServerCertificate))
                        avatarSettings.AvatarApi.ValidateServerCertificate = validateServerCertificate;
                    break;
                #endregion

                #region IdentityAccessSettings
                case Constants.IdentityAccessSettingsEnv.UserDatabasePath:
                    avatarSettings.IdentityAccess.UserDatabasePath = value;
                    break;
                #endregion

                #region CortexLibrarySettings
                case Constants.CortexLibrarySettingsEnv.NeuronsUrl:
                    avatarSettings.CortexLibrary.NeuronsUrl = value;
                    break;
                case Constants.CortexLibrarySettingsEnv.TerminalsUrl:
                    avatarSettings.CortexLibrary.TerminalsUrl = value;
                    break;
                #endregion

                #region CortexDiaryNucleusSettings
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsDatabasePath:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsDatabasePath = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPollingIntervalSecs:
                    int subscriptionsPollingIntervalSecs;
                    if (int.TryParse(value, out subscriptionsPollingIntervalSecs))
                        avatarSettings.CortexDiaryNucleus.SubscriptionsPollingIntervalSecs = subscriptionsPollingIntervalSecs;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushOwner:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsPushOwner = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushPublicKey:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsPushPublicKey = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushPrivateKey:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsPushPrivateKey = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpServerAddress:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpServerAddress = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpPort:
                    int subscriptionsSmtpPort;
                    if (int.TryParse(value, out subscriptionsSmtpPort))
                        avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpPort = subscriptionsSmtpPort;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpUseSsl:
                    bool subscriptionsSmtpUseSsl;
                    if (bool.TryParse(value, out subscriptionsSmtpUseSsl))
                        avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpUseSsl = subscriptionsSmtpUseSsl;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderName:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderName = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderAddress:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderAddress = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderUsername:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderUsername = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderPassword:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderPassword = value;
                    break;
                case Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsCortexGraphOutBaseUrl:
                    avatarSettings.CortexDiaryNucleus.SubscriptionsCortexGraphOutBaseUrl = value;
                    break;
                #endregion

                #region Cortex Chat Nucleus Settings
                case Constants.CortexChatNucleusSettingsEnv.PageSize:
                    if (int.TryParse(value, out int pageSize))
                        avatarSettings.CortexChatNucleus.PageSize = pageSize;
                    break;
                case Constants.CortexChatNucleusSettingsEnv.AppUserId:
                    avatarSettings.CortexChatNucleus.AppUserId = value;
                    break;
                #endregion

                #region Cortex Graph Persistence Settings
                case Constants.CortexGraphPersistenceSettingsEnv.ArangoRootPassword:
                    avatarSettings.CortexGraphPersistence.ArangoRootPassword = value;
                    break;
                #endregion

                default:
                    break;
            }
        }

        return avatarSettings;
    }

    public async Task SaveAsync(AvatarSettings avatarSettings)
    {
        AssertionConcern.AssertArgumentNotNull(avatarSettings, nameof(avatarSettings));

        var path = Path.Combine(this.avatarContextService.Avatar.Id, Constants.Filenames.VariablesEnv);
        var envVariables = new Dictionary<string, string>();

        var envFile = await File.ReadAllLinesAsync(path);

        foreach (var line in envFile)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                continue;

            var index = line.IndexOf('=');

            // If no "=" found, go to the next line
            if (index == -1) continue;

            var key = line[..index].Trim();
            var value = line[(index + 1)..].Trim();

            envVariables[key] = value;
        }

        // Update key-value pairs
        #region EventSourcingSettings
        envVariables[Constants.EventSourcingSettingsEnv.DatabasePath] = avatarSettings.EventSourcing.DatabasePath;
        envVariables[Constants.EventSourcingSettingsEnv.DisplayErrorTraces] = avatarSettings.EventSourcing.DisplayErrorTraces.ToString();
        #endregion

        #region CortexGraphSettingsEnv
        envVariables[Constants.CortexGraphSettingsEnv.PollInterval] = avatarSettings.CortexGraph.PollInterval.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DbName] = avatarSettings.CortexGraph.DbName;
        envVariables[Constants.CortexGraphSettingsEnv.DbUsername] = avatarSettings.CortexGraph.DbUsername;
        envVariables[Constants.CortexGraphSettingsEnv.DbPassword] = avatarSettings.CortexGraph.DbPassword;
        envVariables[Constants.CortexGraphSettingsEnv.DbUrl] = avatarSettings.CortexGraph.DbUrl;
        envVariables[Constants.CortexGraphSettingsEnv.DefaultRelativeValues] = avatarSettings.CortexGraph.DefaultRelativeValues.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultNeuronActiveValues] = avatarSettings.CortexGraph.DefaultNeuronActiveValues.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultTerminalActiveValues] = avatarSettings.CortexGraph.DefaultTerminalActiveValues.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultPageSize] = avatarSettings.CortexGraph.DefaultPageSize.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultPage] = avatarSettings.CortexGraph.DefaultPage.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultDepth] = avatarSettings.CortexGraph.DefaultDepth.ToString();
        envVariables[Constants.CortexGraphSettingsEnv.DefaultDirectionValues] = avatarSettings.CortexGraph.DefaultDirectionValues.ToString();
        #endregion

        #region AvatarApiSettingsEnv
        envVariables[Constants.AvatarApiSettingsEnv.ResourceDatabasePath] = avatarSettings.AvatarApi.ResourceDatabasePath;
        envVariables[Constants.AvatarApiSettingsEnv.RequireAuthentication] = avatarSettings.AvatarApi.RequireAuthentication.ToString();
        envVariables[Constants.AvatarApiSettingsEnv.AnonymousUserId] = avatarSettings.AvatarApi.AnonymousUserId.ToString();
        envVariables[Constants.AvatarApiSettingsEnv.TokenIssuerAddress] = avatarSettings.AvatarApi.TokenIssuerAddress;
        envVariables[Constants.AvatarApiSettingsEnv.ApiName] = avatarSettings.AvatarApi.ApiName;
        envVariables[Constants.AvatarApiSettingsEnv.ApiSecret] = avatarSettings.AvatarApi.ApiSecret;
        envVariables[Constants.AvatarApiSettingsEnv.ValidateServerCertificate] = avatarSettings.AvatarApi.ValidateServerCertificate.ToString();
        #endregion

        #region IdentityAccessSettings
        envVariables[Constants.IdentityAccessSettingsEnv.UserDatabasePath] = avatarSettings.IdentityAccess.UserDatabasePath;
        #endregion

        #region CortexLibrarySettings
        envVariables[Constants.CortexLibrarySettingsEnv.NeuronsUrl] = avatarSettings.CortexLibrary.NeuronsUrl;
        envVariables[Constants.CortexLibrarySettingsEnv.TerminalsUrl] = avatarSettings.CortexLibrary.TerminalsUrl;
        #endregion

        #region CortexDiaryNucleusSettings
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsDatabasePath] = avatarSettings.CortexDiaryNucleus.SubscriptionsDatabasePath;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPollingIntervalSecs] = avatarSettings.CortexDiaryNucleus.SubscriptionsPollingIntervalSecs.ToString();
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushOwner] = avatarSettings.CortexDiaryNucleus.SubscriptionsPushOwner;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushPublicKey] = avatarSettings.CortexDiaryNucleus.SubscriptionsPushPublicKey;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsPushPrivateKey] = avatarSettings.CortexDiaryNucleus.SubscriptionsPushPrivateKey;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpServerAddress] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpServerAddress;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpPort] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpPort.ToString();
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpUseSsl] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpUseSsl.ToString();
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderName] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderName;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderAddress] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderAddress;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderUsername] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderUsername;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsSmtpSenderPassword] = avatarSettings.CortexDiaryNucleus.SubscriptionsSmtpSenderPassword;
        envVariables[Constants.CortexDiaryNucleusSettingsEnv.SubscriptionsCortexGraphOutBaseUrl] = avatarSettings.CortexDiaryNucleus.SubscriptionsCortexGraphOutBaseUrl;
        #endregion

        #region CortexChatNucleusSettings
        envVariables[Constants.CortexChatNucleusSettingsEnv.PageSize] = avatarSettings.CortexChatNucleus.PageSize.ToString();
        envVariables[Constants.CortexChatNucleusSettingsEnv.AppUserId] = avatarSettings.CortexChatNucleus.AppUserId;
        #endregion

        #region CortexGraphPersistenceSettings
        envVariables[Constants.CortexGraphPersistenceSettingsEnv.ArangoRootPassword] = avatarSettings.CortexGraphPersistence.ArangoRootPassword;
        #endregion

        // Write changes to the file
        using var writer = new StreamWriter(path);

        foreach (var keyValuePair in envVariables)
        {
            writer.WriteLine($"{keyValuePair.Key}={keyValuePair.Value}");
        }
    }
}
