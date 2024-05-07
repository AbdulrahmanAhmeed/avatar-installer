﻿namespace ei8.Avatar.Installer.Common
{
    public sealed class Constants
    {
        public sealed class Filenames
        {
            public const string TraefikToml = "traefik.toml";
            public const string SshConfig = "ssh_config";

            public const string StartTraefikBat = "start - traefik.bat";
            public const string StartEi8SiteBat = "start - ei8.site.bat";
            public const string LoopStartEi8SiteBat = "loop start - ei8.site.bat";
            public const string AutoLockCmd = "autolock.cmd";
            public const string VariablesEnv = "variables.env";
        }

        public sealed class EventSourcingSettingsEnv
        {
            public const string DatabasePath = "DATABASE_PATH";
            public const string DisplayErrorTraces = "DISPLAY_ERROR_TRACES";
        }

        public sealed class CortexGraphSettingsEnv
        {
            public const string PollInterval = "POLL_INTERVAL";
            public const string DbName = "DB_NAME";
            public const string DbUsername = "DB_USERNAME";
            public const string DbPassword = "DB_PASSWORD";
            public const string DbUrl = "DB_URL";
            public const string DefaultRelativeValues = "DEFAULT_RELATIVE_VALUES";
            public const string DefaultNeuronActiveValues = "DEFAULT_NEURON_ACTIVE_VALUES";
            public const string DefaultTerminalActiveValues = "DEFAULT_TERMINAL_ACTIVE_VALUES";
            public const string DefaultPageSize = "DEFAULT_PAGE_SIZE";
            public const string DefaultPage = "DEFAULT_PAGE";
            public const string ArangoRootPassword = "ARANGO_ROOT_PASSWORD";
        }

        public sealed class AvatarApiSettingsEnv
        {
            public const string ResourceDatabasePath = "RESOURCE_DATABASE_PATH";
            public const string RequireAuthentication = "REQUIRE_AUTHENTICATION";
            public const string AnonymousUserId = "ANONYMOUS_USER_ID";
            public const string ProxyUserId = "PROXY_USER_ID";
            public const string TokenIssuerUrl = "TOKEN_ISSUER_URL";
            public const string ApiName = "API_NAME";
            public const string ApiSecret = "API_SECRET";
            public const string ValidateServerCertificate = "VALIDATE_SERVER_CERTIFICATE";
        }

        public sealed class IdentityAccessSettingsEnv
        {
            public const string UserDatabasePath = "USER_DATABASE_PATH";
        }

        public sealed class CortexLibrarySettingsEnv
        {
            public const string NeuronsUrl = "NEURONS_URL";
            public const string TerminalsUrl = "TERMINALS_URL";
        }

        public sealed class CortexDiaryNucleusSettingsEnv
        {
            public const string SubscriptionsDatabasePath = "SUBSCRIPTIONS_DATABASE_PATH";
            public const string SubscriptionsPollingIntervalSecs = "SUBSCRIPTIONS_POLLING_INTERVAL_SECS";
            public const string SubscriptionsPushOwner = "SUBSCRIPTIONS_PUSH_OWNER";
            public const string SubscriptionsPushPublicKey = "SUBSCRIPTIONS_PUSH_PUBLIC_KEY";
            public const string SubscriptionsPushPrivateKey = "SUBSCRIPTIONS_PUSH_PRIVATE_KEY";
            public const string SubscriptionsSmtpServerAddress = "SUBSCRIPTIONS_SMTP_SERVER_ADDRESS";
            public const string SubscriptionsSmtpPort = "SUBSCRIPTIONS_SMTP_PORT";
            public const string SubscriptionsSmtpUseSsl = "SUBSCRIPTIONS_SMTP_USE_SSL";
            public const string SubscriptionsSmtpSenderName = "SUBSCRIPTIONS_SMTP_SENDER_NAME";
            public const string SubscriptionsSmtpSenderAddress = "SUBSCRIPTIONS_SMTP_SENDER_ADDRESS";
            public const string SubscriptionsSmtpSenderUsername = "SUBSCRIPTIONS_SMTP_SENDER_USERNAME";
            public const string SubscriptionsSmtpSenderPassword = "SUBSCRIPTIONS_SMTP_SENDER_PASSWORD";
            public const string SubscriptionsCortexGraphOutBaseUrl = "SUBSCRIPTIONS_CORTEX_GRAPH_OUT_BASE_URL";
        }

        public sealed class d23SettingsEnv
        {
            public const string OidcAuthority = "OIDC_AUTHORITY";
            public const string ClientId = "CLIENT_ID";
            public const string ClientSecret = "CLIENT_SECRET";
            public const string UpdateCheckInterval = "UPDATE_CHECK_INTERVAL";
            public const string DatabasePath = "DATABASE_PATH";
            public const string BasePath = "BASE_PATH";
            public const string PluginsPath = "PLUGINS_PATH";
            public const string ValidateServerCertificate = "VALIDATE_SERVER_CERTIFICATE";
            public const string AppTitle = "APP_TITLE";
            public const string AppIcon = "APP_ICON";
        }

        public sealed class AvatarNetworkSettingsEnv
        {
            public const string AvatarIp = "AVATAR_IP";
            public const string D23Ip = "D23_IP";
            public const string AvatarInPort = "AVATAR_IN_PORT";
            public const string D23BlazorPort = "D23_BLAZOR_PORT";
        }

        public sealed class Databases
        {
            public const string AvatarDb = "avatar.db";
            public const string d23Db = "d23.db";
            public const string EventsDb = "events.db";
            public const string IdentityAccessDb = "identity-access.db";
            public const string SubscriptionsDb = "subscriptions.db";
        }

        public sealed class TableNames
        {
            public const string NeuronPermit = "NeuronPermit";
            public const string RegionPermit = "RegionPermit";
            public const string User = "User";
        }

        public sealed class Titles
        {
            public const string AvatarInstaller = "Avatar Installer";
            public const string NeuronPermit = "Neuron Permit";
            public const string RegionPermit = "Region Permit";
            public const string User = "User";
            public const string EventSourcingSettings = "Event Sourcing Settings";
            public const string CortexGraphSettings = "Cortex Graph Settings";
            public const string AvatarApiSettings = "Avatar Api Settings";
        }

        public sealed class Statuses
        {
            public const string Cancelled = "Cancelled";
            public const string Success = "Success";
            public const string Error = "Error";
            public const string Invalid = "Invalid";
            public const string Save = "Save";
            public const string Remove = "Remove";
            public const string Add = "Add";
        }

        public sealed class Prompts
        {
            public const string Ok = "Ok";
            public const string Yes = "Yes";
            public const string No = "No";
        }

        public sealed class Operations
        {
            public const string Create = "create";
            public const string Created = "created";
            public const string Get = "get";
            public const string Save = "save";
            public const string Saved = "saved";
            public const string Remove = "remove";
            public const string Removed = "removed";
            public const string Empty = "empty";
        }

        public sealed class Messages
        {
            public const string InvalidConfig = "Configuration must be a json file";
            public const string ChooseConfig = "Please choose a configuration file";
            public const string AvatarInstalled = "Avatar has been successfully installed";
            public const string EditingCancelled = "Editing avatar cancelled";
            public const string Confirmation = "Are you sure you want to {0} this {1}?";
            public const string Success = "Successfully {0} {1}";
            public const string Error = "Unable to {0} {1}";
            public const string CantBe = "{0} cannot be {1}";
            public const string MustBe = "{0} must be a {1}";
            public const string NotFound = "{0} not found";
            public const string AlreadyExists = "{0} already exists";
        }

        public sealed class Urls
        {
            public const string DefaultTemplateDownloadUrl = "https://github.com/ei8/avatar-template.git";
        }

        public sealed class BatchFileTemplates
        {
            public const string StartTraefikBat = @"traefik_windows-amd64.exe -c {0}";
            public const string StartEi8SiteBat = "@echo off\n" +
                                                  "@echo ___________________________________________\n" +
                                                  "@echo Started: %date% %time%\n" +
                                                  "ssh -F {0} ei8.site";

            public const string LoopStartEi8SiteBat = "@echo off\n" +
                                                      "@echo Loop-starting ... \n" +
                                                     @"FOR /L %%N IN () DO call ""start - ei8.site.bat""";

            public const string AutoLockCmd = "timeout /t 10\n" +
                                               "rundll32.exe user32.dll,LockWorkStation";
        }
    }
}
