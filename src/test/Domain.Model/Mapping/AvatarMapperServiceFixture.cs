using AutoMapper;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.Domain.Model.Avatars.Settings;
using ei8.Avatar.Installer.Domain.Model.Configuration;
using ei8.Avatar.Installer.Domain.Model.Mapping;
using Microsoft.Extensions.Logging.Abstractions;

namespace Domain.Model.Test.Mapping
{
    public class AvatarMapperServiceFixture
    {
        private readonly MapperConfiguration mapperConfig;
        private readonly IMapper mapper;

        // setup
        public AvatarMapperServiceFixture()
        {
            this.mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AvatarAutoMapperProfile>();
            },
                new NullLoggerFactory()
            );

            this.mapper = new Mapper(mapperConfig);
        }

        [Fact]
        public void MapsRoutingConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        Routing = new()
                        {
                            neurULServerDomainName = "www.neurul.net"
                        }
                    },
                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);

            var sampleTarget = new AvatarItem("id_sample1");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("www.neurul.net", sampleResult.RoutingSettings.neurULServerDomainName);

            var defaultTarget = new AvatarItem("id_defaults1");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("fibona.cc", defaultResult.RoutingSettings.neurULServerDomainName);
        }

        [Fact]
        public void MapsCortexGraphPersistenceConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
            new()
            {
                CortexGraphPersistence = new()
                {
                    ArangoRootPassword = "test",
                }
            },
            new()
                }
            };

            var sut = new AvatarMapperService(mapper);

            var sampleTarget = new AvatarItem("id_sample1");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("test", sampleResult.Settings.CortexGraphPersistence.ArangoRootPassword);

            var defaultTarget = new AvatarItem("id_defaults1");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("", defaultResult.Settings.CortexGraphPersistence.ArangoRootPassword);
        }

        [Fact]
        public void MapsCortexGraphConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        CortexGraph = new()
                        {
                            DbName = "custom",
                            DbUrl = @"http://www.example.com",
                            DbUsername = "not-root"
                        }
                    },
                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);

            var sampleTarget = new AvatarItem("id_sample1");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("custom", sampleResult.Settings.CortexGraph.DbName);
            Assert.Equal(@"http://www.example.com", sampleResult.Settings.CortexGraph.DbUrl);
            Assert.Equal("not-root", sampleResult.Settings.CortexGraph.DbUsername);

            var defaultTarget = new AvatarItem("id_defaults1");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("graph_${AVATAR_NAME}", defaultResult.Settings.CortexGraph.DbName);
            Assert.Equal(@"http://cortex.graph.persistence:8529", defaultResult.Settings.CortexGraph.DbUrl);
            Assert.Equal("root", defaultResult.Settings.CortexGraph.DbUsername);
        }

        [Fact]
        public void MapsAvatarApiConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        AvatarApi = new()
                        {
                            ApiName = "api",
                            TokenIssuerAddress = @"https://www.junvic.me",
                            AnonymousUserId = "Guestotototto"
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample2");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("api", sampleResult.Settings.AvatarApi.ApiName);
            Assert.Equal(@"https://www.junvic.me", sampleResult.Settings.AvatarApi.TokenIssuerAddress);
            Assert.Equal("Guestotototto", sampleResult.Settings.AvatarApi.AnonymousUserId);


            var defaultTarget = new AvatarItem("id_defaults2");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("avatarapi-${AVATAR_NAME}", defaultResult.Settings.AvatarApi.ApiName);
            Assert.Equal(@"https://login.fibona.cc", defaultResult.Settings.AvatarApi.TokenIssuerAddress);
        }

        [Fact]
        public void MapsCortexLibraryConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        CortexLibrary = new()
                        {
                            NeuronsUrl = @"https://neuronsurl.example.com",
                            TerminalsUrl = @"https://terminalsurl.example.com"
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample3");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal(@"https://neuronsurl.example.com", sampleResult.Settings.CortexLibrary.NeuronsUrl);
            Assert.Equal(@"https://terminalsurl.example.com", sampleResult.Settings.CortexLibrary.TerminalsUrl);


            var defaultTarget = new AvatarItem("id_defaults3");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal(@"https://fibona.cc/${AVATAR_NAME}/cortex/neurons", defaultResult.Settings.CortexLibrary.NeuronsUrl);
            Assert.Equal(@"https://fibona.cc/${AVATAR_NAME}/cortex/terminals", defaultResult.Settings.CortexLibrary.TerminalsUrl);
        }

        [Fact]
        public void MapsEventSourcingConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        EventSourcing = new()
                        {
                            PrivateKeyPath = @"my/path",
                            InProcessPrivateKeyPath = @"my/inprocesspath",
                            EncryptionEnabled = true,
                            EncryptedEventsKey = "enckey"
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample3");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal(@"my/path", sampleResult.Settings.EventSourcing.PrivateKeyPath);
            Assert.Equal(@"my/inprocesspath", sampleResult.Settings.EventSourcing.InProcessPrivateKeyPath);
            Assert.True(sampleResult.Settings.EventSourcing.EncryptionEnabled);
            Assert.Equal(@"enckey", sampleResult.Settings.EventSourcing.EncryptedEventsKey);

            var defaultTarget = new AvatarItem("id_defaults3");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("/C/keys/private.key", defaultResult.Settings.EventSourcing.PrivateKeyPath);
            Assert.Equal(string.Empty, defaultResult.Settings.EventSourcing.InProcessPrivateKeyPath);
            Assert.False(defaultResult.Settings.EventSourcing.EncryptionEnabled);
            Assert.Equal(string.Empty, defaultResult.Settings.EventSourcing.EncryptedEventsKey);
        }

        [Fact]
        public void MapsUn8yConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        Un8y = new()
                        {
                            OidcAuthorityUrl = @"https://www.example.com",
                            ClientId = "not-sample",
                            RequestedScopes = "new scope",
                            BasePath = "/",
                            CertificatePassword = "certpassword",
                            CertificatePath = "/cert/path"
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample4");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal(@"https://www.example.com", sampleResult.Un8ySettings.OidcAuthority);
            Assert.Equal("not-sample", sampleResult.Un8ySettings.ClientId);
            Assert.Equal("new scope", sampleResult.Un8ySettings.RequestedScopes);
            Assert.Equal("/", sampleResult.Un8ySettings.BasePath);
            Assert.Equal("certpassword", sampleResult.Un8ySettings.CertificatePassword);
            Assert.Equal("/cert/path", sampleResult.Un8ySettings.CertificatePath);

            var defaultTarget = new AvatarItem("id_defaults4");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal(@"https://login.fibona.cc", defaultResult.Un8ySettings.OidcAuthority);
            Assert.Equal("un8y-${AVATAR_NAME}", defaultResult.Un8ySettings.ClientId);
            Assert.Equal("openid,profile,email,avatarapi-${AVATAR_NAME},offline_access", defaultResult.Un8ySettings.RequestedScopes);
            Assert.Equal("/${AVATAR_NAME}/un8y", defaultResult.Un8ySettings.BasePath);
            Assert.Equal(string.Empty, defaultResult.Un8ySettings.CertificatePassword);
            Assert.Equal("/https/aspnetapp.pfx", defaultResult.Un8ySettings.CertificatePath);
        }

        [Fact]
        public void MapOrchestrationConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        Orchestration = new()
                        {
                            AvatarName = "myavatar",
                            TunnelLocalPort = 9999,
                            KeysPath = "my/keys"
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample5");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("myavatar", sampleResult.OrchestrationSettings.AvatarName);
            Assert.Equal(9999, sampleResult.OrchestrationSettings.TunnelLocalPort);
            Assert.Equal("my/keys", sampleResult.OrchestrationSettings.KeysPath);

            var defaultTarget = new AvatarItem("id_defaults5");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("sample", defaultResult.OrchestrationSettings.AvatarName);
            Assert.Equal(9393, defaultResult.OrchestrationSettings.TunnelLocalPort);
            Assert.Equal(string.Empty, defaultResult.OrchestrationSettings.KeysPath);
        }

        [Fact]
        public void MapsCortexChatNucleusConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        OwnerUserId = "jono",
                        CortexChatNucleus  = new()
                        {
                            PageSize = 6,
                            InitializeMissingMirrors = false
                        }
                    },

                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample5");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal(6, sampleResult.Settings.CortexChatNucleus.PageSize);
            Assert.Equal("jono", sampleResult.Settings.CortexChatNucleus.AppUserId);
            Assert.False(sampleResult.Settings.CortexChatNucleus.InitializeMissingMirrors);

            var defaultTarget = new AvatarItem("id_defaults5");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal(10, defaultResult.Settings.CortexChatNucleus.PageSize);
            Assert.Null(defaultResult.Settings.CortexChatNucleus.AppUserId);
            Assert.True(defaultResult.Settings.CortexChatNucleus.InitializeMissingMirrors);
        }

        [Fact]
        public void MapsSshConfiguration()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[2]
                {
                    new()
                    {
                        OwnerUserId = "jono",
                        Orchestration = new()
                        {
                            AvatarName = "ssher",
                            TunnelLocalPort = 88
                        },
                        Ssh = new()
                        {
                            HostName = "ei8hoster.com",
                            Port = 93,
                            ServerAliveCountMax = 77,
                            ServerAliveInterval = 3
                        }
                    },
                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample5");
            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("ssher:80 localhost:88", sampleResult.Settings.Ssh.RemoteForward);
            Assert.Equal("ei8hoster.com", sampleResult.Settings.Ssh.HostName);
            Assert.Equal(93, sampleResult.Settings.Ssh.Port);
            Assert.Equal(77, sampleResult.Settings.Ssh.ServerAliveCountMax);
            Assert.Equal(3, sampleResult.Settings.Ssh.ServerAliveInterval);

            var defaultTarget = new AvatarItem("id_defaults5");
            var defaultResult = sut.Apply(config.Avatars[1], defaultTarget);

            Assert.Equal("sample:80 localhost:9393", defaultResult.Settings.Ssh.RemoteForward);
            Assert.Equal("ei8.host", defaultResult.Settings.Ssh.HostName);
            Assert.Equal(2222, defaultResult.Settings.Ssh.Port);
            Assert.Equal(60 * 24 * 365, defaultResult.Settings.Ssh.ServerAliveCountMax);
            Assert.Equal(60, defaultResult.Settings.Ssh.ServerAliveInterval);
        }

        [Fact]
        public void DoesNotOverwritePropertiesNotConfigured()
        {
            var config = new AvatarServerConfiguration()
            {
                Avatars = new AvatarConfigurationItem[1]
                {
                    new()
                }
            };

            var sut = new AvatarMapperService(mapper);
            var sampleTarget = new AvatarItem("id_sample6")
            {
                Settings = new AvatarSettings
                {
                    EventSourcing = new EventSourcingSettings()
                    {
                        DatabasePath = "/C/db/events.db",
                        DisplayErrorTraces = false
                    },
                    CortexGraph = new CortexGraphSettings()
                    {
                        PollInterval = 1000,
                        DefaultRelativeValues = 3,
                        DefaultNeuronActiveValues = 1,
                        DefaultTerminalActiveValues = 1,
                        DefaultPage = 1,
                        DefaultPageSize = 10
                    },
                    AvatarApi = new AvatarApiSettings()
                    {
                        ResourceDatabasePath = "/C/db/avatar.db",
                        RequireAuthentication = true,
                        AnonymousUserId = "Gusto",
                        ProxyUserId = Guid.Parse("ca008cde-61bb-4260-92fb-9abcca1209ef"),
                        ApiSecret = "72019790-7daa-4de6-a7a9-3a1615cabf59",
                        ValidateServerCertificate = false
                    },
                    IdentityAccess = new IdentityAccessSettings()
                    {
                        UserDatabasePath = "/C/db/identity-access.db"
                    },
                    CortexDiaryNucleus = new CortexDiaryNucleusSettings()
                    {
                        SubscriptionsDatabasePath = "/C/db/subscriptions.db",
                        SubscriptionsPollingIntervalSecs = 15,
                        SubscriptionsPushOwner = "mailto:example@example.com",
                        SubscriptionsPushPublicKey = "aLrW80tp5imbJlxAY5WJnmtzaZvTCJoM8ywZEI6E65VTHcqtB69tnqUsRkYC6U-1WfSj0bFovZF6DZaA9Bgo0Ts",
                        SubscriptionsPushPrivateKey = "JlI0-oL2NQ8HyAAZF3pGxWtbjRsYQxTB9tg6Fe7_1x8",
                        SubscriptionsSmtpServerAddress = "smtp.gmail.com",
                        SubscriptionsSmtpPort = 587,
                        SubscriptionsSmtpUseSsl = false,
                        SubscriptionsSmtpSenderName = "ei8 Support",
                        SubscriptionsSmtpSenderAddress = "support@ei8.works",
                        SubscriptionsSmtpSenderUsername = "support@ei8.works",
                        SubscriptionsSmtpSenderPassword = string.Empty,
                        SubscriptionsCortexGraphOutBaseUrl = "http://cortex.graph.out.api:80"
                    },
                    CortexChatNucleus = new CortexChatNucleusSettings()
                    {
                        PageSize = 8,
                        AppUserId = "me2@me2.com"
                    }
                }
            };

            var sampleResult = sut.Apply(config.Avatars[0], sampleTarget);

            Assert.Equal("/C/db/events.db", sampleResult.Settings.EventSourcing.DatabasePath);
            Assert.Equal(1000, sampleResult.Settings.CortexGraph.PollInterval);
            Assert.Equal("Guest", sampleResult.Settings.AvatarApi.AnonymousUserId);
            Assert.Equal("/C/db/identity-access.db", sampleResult.Settings.IdentityAccess.UserDatabasePath);
            Assert.Equal("aLrW80tp5imbJlxAY5WJnmtzaZvTCJoM8ywZEI6E65VTHcqtB69tnqUsRkYC6U-1WfSj0bFovZF6DZaA9Bgo0Ts", sampleResult.Settings.CortexDiaryNucleus.SubscriptionsPushPublicKey);
        }
    }
}
