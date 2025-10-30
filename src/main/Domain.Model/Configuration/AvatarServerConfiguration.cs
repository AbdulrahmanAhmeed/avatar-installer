using System.Text.Json.Serialization;

namespace ei8.Avatar.Installer.Domain.Model.Configuration
{
    /// <summary>
    /// Defines how an <see cref="Avatars.AvatarItem"/> should be configured
    /// </summary>
    public class AvatarServerConfiguration
    {
        public string ServerName { get; set; }
        public AvatarConfigurationItem[] Avatars { get; set; }
        public string Destination { get; set; }
        public string TemplateUrl { get; set; }
        public AvatarServerSshConfiguration Network { get; set; }

        [JsonConstructor]
        public AvatarServerConfiguration(string serverName)
        {
            this.ServerName = serverName;
            this.Network = new(serverName);
        }
    }

    public class AvatarServerSshConfiguration
    {
        public string LocalIp { get; set; }
        public SshConfiguration Ssh { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public AvatarServerSshConfiguration(string serverName)
        {
            LocalIp = "192.168.1.110";
            Ssh = new(serverName);
        }

        [JsonConstructor]
        public AvatarServerSshConfiguration() : this("sample") { }
    }

    public class AvatarConfigurationItem
    {
        /// <summary>
        /// The folder name of the Avatar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the neurULized Avatar instance.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The user id which is mapped to the neurULized Avatar instance in Iden8y.
        /// </summary>
        public string OwnerUserId { get; set; }
        public RoutingConfiguration Routing { get; set; }
        public EventSourcingConfiguration EventSourcing { get; set; }
        public CortexGraphPersistenceConfiguration CortexGraphPersistence { get; set; }
        public CortexGraphConfiguration CortexGraph { get; set; }
        public AvatarApiConfiguration AvatarApi { get; set; }
        public CortexLibraryConfiguration CortexLibrary { get; set; }
        public Un8yConfiguration Un8y { get; set; }
        public OrchestrationConfiguration Orchestration { get; set; }
        public CortexChatNucleusConfiguration CortexChatNucleus { get; set; }

        [JsonConstructor]
        public AvatarConfigurationItem(string name, string ownerUserId)
        {
            this.Name = name;
            this.OwnerUserId = ownerUserId;
            this.Routing = new();
            this.EventSourcing = new();
            this.CortexGraphPersistence = new();
            this.CortexGraph = new(this.Name);
            this.AvatarApi = new(this.Name);
            this.CortexLibrary = new(this.Name);
            this.Un8y = new(this.Name);
            this.Orchestration = new();
            this.CortexChatNucleus = new(this.OwnerUserId);
        }
    }

    public class EventSourcingConfiguration
    {
        public string PrivateKeyPath { get; set; }
        public string InProcessPrivateKeyPath { get; set; }
        public bool EncryptionEnabled { get; set; }
        public string EncryptedEventsKey { get; set; }

        public EventSourcingConfiguration()
        {
            this.PrivateKeyPath = "/C/keys/private.key";
            this.InProcessPrivateKeyPath = string.Empty;
            this.EncryptionEnabled = false;
            this.EncryptedEventsKey = string.Empty;
        }
    }

    public class CortexGraphPersistenceConfiguration
    {
        public string ArangoRootPassword { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public CortexGraphPersistenceConfiguration()
        {
            this.ArangoRootPassword = string.Empty;
        }
    }

    public class CortexGraphConfiguration
    {
        public string DbName { get; set; }
        public string DbUsername { get; set; }
        public string DbUrl { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public CortexGraphConfiguration(string avatarName)
        {
            DbName = $"graph_{avatarName}";
            DbUsername = "root";
            DbUrl = "http://cortex.graph.persistence:8529";
        }

        [JsonConstructor]
        public CortexGraphConfiguration() : this("sample")
        {
        }
    }

    public class AvatarApiConfiguration
    {
        public string AnonymousUserId { get; set; }
        public string TokenIssuerAddress { get; set; }
        public string ApiName { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public AvatarApiConfiguration(string avatarName)
        {
            AnonymousUserId = "Guest";
            TokenIssuerAddress = @"https://login.fibona.cc";
            ApiName = $"avatarapi-{avatarName}";
        }

        [JsonConstructor]
        public AvatarApiConfiguration() : this("sample") { }
    }

    public class CortexLibraryConfiguration
    {
        public string NeuronsUrl { get; set; }
        public string TerminalsUrl { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public CortexLibraryConfiguration(string avatarName)
        {
            NeuronsUrl = $@"https://fibona.cc/{avatarName}/cortex/neurons";
            TerminalsUrl = $@"https://fibona.cc/{avatarName}/cortex/terminals";
        }

        [JsonConstructor]
        public CortexLibraryConfiguration() : this("sample") { }
    }

    public class Un8yConfiguration
    {
        public string OidcAuthorityUrl { get; set; }
        public string ClientId { get; set; }
        public string RequestedScopes { get; set; }
        public string BasePath { get; set; }
        public string CertificatePassword { get; set; }
        public string CertificatePath { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public Un8yConfiguration(string avatarName)
        {
            OidcAuthorityUrl = @"https://login.fibona.cc";
            ClientId = $"un8y-{avatarName}";
            RequestedScopes = $"openid,profile,email,avatarapi-{avatarName},offline_access";
            BasePath = $"/{avatarName}/un8y";
            CertificatePassword = string.Empty;
            CertificatePath = "/https/aspnetapp.pfx";
        }

        [JsonConstructor]
        public Un8yConfiguration() : this("sample") { }
    }

    public class RoutingConfiguration
    {
        public string neurULServerDomainName { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>

        [JsonConstructor]
        public RoutingConfiguration()
        {
            neurULServerDomainName = "fibona.cc";
        }
    }

    public class OrchestrationConfiguration
    {
        public string AvatarIp { get; set; }
        public int AvatarInPort { get; set; }
        public string Un8yIp { get; set; }
        public int Un8yBlazorPort { get; set; }
        public string KeysPath { get; set; }
        public int GraphPersistencePort { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>

        [JsonConstructor]
        public OrchestrationConfiguration()
        {
            AvatarIp = "192.168.1.110";
            AvatarInPort = 64101;
            Un8yIp = "192.168.1.110";
            Un8yBlazorPort = 64103;
            KeysPath = string.Empty;
            GraphPersistencePort = 8529;
        }
    }

    public class CortexChatNucleusConfiguration
    {
        public int PageSize { get; set; }
        public string AppUserId { get; set; }
        public bool CreateExternalReferencesIfNotFound { get; set; }

        public CortexChatNucleusConfiguration(string appUserId)
        {
            this.PageSize =  10;
            this.AppUserId = appUserId;
            this.CreateExternalReferencesIfNotFound = true;
        }

        [JsonConstructor]
        public CortexChatNucleusConfiguration() : this(string.Empty) { }
    }

    public class SshConfiguration
    {
        public int ServerAliveInterval { get; set; }
        public int ServerAliveCountMax { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public string RemoteForward { get; set; }
        public int LocalPort { get; set; }

        /// <summary>
        /// Initialize with defaults
        /// </summary>
        public SshConfiguration(string serverName)
        {
            ServerAliveInterval = 60;            // 1 minute
            ServerAliveCountMax = 60 * 24 * 365; // 60 secs * 24 hours * 365 days = 1 year
            Port = 2222;
            HostName = "ei8.host";
            RemoteForward = $"{serverName}:80";
            LocalPort = 9393;
        }

        [JsonConstructor]
        public SshConfiguration() : this("sample") { }
    }
}
