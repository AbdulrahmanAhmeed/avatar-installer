using ei8.Avatar.Installer.Common;
using ei8.Avatar.Installer.Domain.Model;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.IO.Process.Services.Settings;
using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23.neurULization;
using ei8.Cortex.Coding.d23.neurULization.Implementation;
using ei8.Cortex.Coding.d23.neurULization.Persistence;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Cortex.Coding.Persistence;
using ei8.EventSourcing.Application;
using ei8.EventSourcing.Client;
using ei8.EventSourcing.Domain.Model;
using ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite;
using ei8.Extensions.DependencyInjection.Coding;
using ei8.Extensions.DependencyInjection.Coding.d23.neurULization;
using ei8.Extensions.DependencyInjection.Cortex;
using ei8.Extensions.DependencyInjection.EventSourcing;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy.TinyIoc;
using neurUL.Common;
using neurUL.Common.Domain.Model;
using System.Reflection;
using System.Text.Json;

namespace ei8.Avatar.Installer.IO.Process.Services.Avatars
{
    public class AvatarItemWriteRepository : IAvatarItemWriteRepository
    {
        private readonly ILogger<AvatarItemWriteRepository> logger;

        public AvatarItemWriteRepository(
            ILogger<AvatarItemWriteRepository> logger
        )
        {
            this.logger = logger;
        }

        private List<string> SerializeEnvironmentVariables<T>(T settings)
            where T : class, new()
        {
            if (settings == null)
                return null;

            var lines = new List<string>();

            foreach (var property in settings.GetType().GetProperties())
            {
                var ieva = property.GetCustomAttributes<IgnoreEnvironmentVariableAttribute>();

                if (ieva.Any())
                    continue;

                var evka = property.GetCustomAttributes<EnvironmentVariableKeyAttribute>();

                var environmentVariableKey = evka.SingleOrDefault() == null ? property.Name.ToMacroCase() : evka.SingleOrDefault().Key;
                var environmentVariableValue = property.GetValue(settings);

                if (property == typeof(bool))
                    environmentVariableValue = ((string)environmentVariableValue).ToLower();

                lines.Add($"{environmentVariableKey}={environmentVariableValue}");
            }

            return lines;
        }

        public async Task SaveAsync(AvatarItem avatarItem)
        {
            await SaveEnvironmentVariablesAsync(avatarItem);
            await CreateSqliteDatabasesAsync(avatarItem);
        }

        private async Task SaveEnvironmentVariablesAsync(AvatarItem avatarItem)
        {
            logger.LogInformation("Serializing variables.env");
            var variablesLines = new List<string>();
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.EventSourcing));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.CortexGraph));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.AvatarApi));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.IdentityAccess));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.CortexLibrary));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.CortexDiaryNucleus));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.CortexChatNucleus));
            variablesLines.AddRange(SerializeEnvironmentVariables(avatarItem.Settings.CortexGraphPersistence));
            await File.WriteAllLinesAsync(Path.Combine(avatarItem.Id, Common.Constants.Filenames.VariablesEnv), variablesLines);

            logger.LogInformation("Serializing un8y/variables.env");
            var un8yLines = SerializeEnvironmentVariables(avatarItem.Un8ySettings);
            await File.WriteAllLinesAsync(Path.Combine(avatarItem.Id, Common.Constants.Directories.Un8y, Common.Constants.Filenames.VariablesEnv), un8yLines);

            logger.LogInformation("Serializing .env");
            var envLines = SerializeEnvironmentVariables(avatarItem.OrchestrationSettings);
            await File.WriteAllLinesAsync(Path.Combine(avatarItem.Id, Common.Constants.Filenames.Env), envLines);
        }

        private async Task CreateSqliteDatabasesAsync(AvatarItem avatarItem)
        {
            // Save the original directory for later use
            // Maui's current directory: "C:\\WINDOWS\\system32"
            // CLI's current directory: "avatar-installer\\src\\main\\Port.Adapter\\UI\\CLI\\bin\\Debug\\net6.0"
            var originalDirectory = Directory.GetCurrentDirectory();

            // Get the directory where the compiled executable (.exe) is located to fix directory issues
            var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(exeDirectory);

            var authorNeuronId = Guid.NewGuid();
            var anonymousNeuronId = Guid.NewGuid();

            foreach (var sqlFile in Directory.EnumerateFiles("./Avatars", "*.sql").OrderBy(f => f))
            {
                logger.LogInformation("Creating database for {sqlFile}", sqlFile);

                var sqlStatements = File.ReadAllText(sqlFile);
                string filenameExcludingIndex = sqlFile.Substring(sqlFile.IndexOf('_') + 1);
                string filenameWithDirectory = filenameExcludingIndex.Replace('_', '/');
                string directory = string.Empty;
                string filename = filenameWithDirectory;
                if (filenameWithDirectory.Contains('/'))
                {
                    directory = filenameWithDirectory.Split('/')[0];
                    filename = filenameWithDirectory.Split('/')[1];
                }
                var sqliteFileName = $"{Path.GetFileNameWithoutExtension(filename)}.db";

                string sqliteFullPath = Path.Combine(avatarItem.Id, directory, sqliteFileName);
                using var connection = new SqliteConnection($@"Data Source=file:{sqliteFullPath}");

                connection.Open();

                if (sqliteFullPath.EndsWith(Common.Constants.Databases.Iden8yDb))
                {
                    string sqlInsertionCommand = @"
INSERT OR IGNORE INTO ""RegionPermit"" (""SequenceId"", ""UserNeuronId"", ""WriteLevel"", ""ReadLevel"")
VALUES 
    (@SequenceId, @UserNeuronId, @WriteLevel, @ReadLevel),
    (@SequenceId2, @UserNeuronId2, @WriteLevel2, @ReadLevel2);

INSERT OR IGNORE INTO ""User"" (""UserId"", ""NeuronId"")
VALUES 
    (@UserId, @NeuronId),
    (@UserId2, @NeuronId2);

COMMIT;
";
                    using (var identityCommand = new SqliteCommand(sqlStatements + sqlInsertionCommand, connection))
                    {
                        identityCommand.Parameters.AddWithValue("@SequenceId", 1);
                        identityCommand.Parameters.AddWithValue("@UserNeuronId", authorNeuronId.ToString());
                        identityCommand.Parameters.AddWithValue("@WriteLevel", 1);
                        identityCommand.Parameters.AddWithValue("@ReadLevel", 1);

                        identityCommand.Parameters.AddWithValue("@SequenceId2", 2);
                        identityCommand.Parameters.AddWithValue("@UserNeuronId2", anonymousNeuronId.ToString());
                        identityCommand.Parameters.AddWithValue("@WriteLevel2", 0);
                        identityCommand.Parameters.AddWithValue("@ReadLevel2", 1);

                        identityCommand.Parameters.AddWithValue("@UserId", avatarItem.OwnerUserId);
                        identityCommand.Parameters.AddWithValue("@NeuronId", authorNeuronId.ToString());

                        identityCommand.Parameters.AddWithValue("@UserId2", avatarItem.Settings.AvatarApi.AnonymousUserId);
                        identityCommand.Parameters.AddWithValue("@NeuronId2", anonymousNeuronId.ToString());

                        await identityCommand.ExecuteNonQueryAsync();
                    }
                }
                else if (sqliteFullPath.EndsWith(Common.Constants.Databases.EventsDb))
                {
                    using (var eventsCommand = new SqliteCommand(sqlStatements, connection))
                        await eventsCommand.ExecuteNonQueryAsync();

                    var container = new TinyIoCContainer();

                    string av8rSettingsPath = Path.Combine(
                        avatarItem.Id,
                        Common.Constants.Directories.Av8r,
                        Common.Constants.Filenames.SettingsJson
                    );

                    var settings = JsonSerializer.Deserialize<SettingsService>(File.ReadAllText(av8rSettingsPath));
                    container.Register(settings.Mirrors);
                    container.Register(Options.Create(container.Resolve<IEnumerable<MirrorConfig>>().ToList()));

                    // Add DataProtector for EventStore
                    var services = new ServiceCollection();
                    services.AddDataProtection();
                    var sp = services.BuildServiceProvider();

                    container.Register(sp.GetRequiredService<IDataProtectionProvider>().CreateProtector(typeof(EventStore).FullName));
                    container.AddInProcessTransactions();
                    container.Register<IKeyService, KeyService>();
                    var ss = container.Resolve<ISettingsService>();
                    ss.EncryptionEnabled = avatarItem.Settings.EventSourcing.EncryptionEnabled;

                    if (ss.EncryptionEnabled)
                    {
                        AssertionConcern.AssertStateTrue(
                            IOHelper.IsPathValidRootedLocal(avatarItem.Settings.EventSourcing.InProcessPrivateKeyPath),
                            "A valid 'InProcessPrivateKeyPath' is required if encryption is enabled."
                        );

                        ss.PrivateKeyPath = avatarItem.Settings.EventSourcing.InProcessPrivateKeyPath;
                        await container.Resolve<IKeyService>().Load(
                            avatarItem.Settings.EventSourcing.EncryptedEventsKey,
                            ss.PrivateKeyPath,
                            ss,
                            ss.GetKeyPropertyPair()
                        );
                    }
                    container.Register<IMirrorRepository, InProcessMirrorRepository>();
                    container.AddDataAdapters();
                    container.Register<INetworkTransactionData, NetworkTransactionData>();
                    container.AddReadWriteCache();
                    container.Register<INetworkTransactionService, NetworkTransactionService>();

                    container.Resolve<ISettingsService>().DatabasePath = sqliteFullPath;

                    // initialize author neuron
                    var nn = new Network();

                    // author neuron
                    nn.AddReplace(Neuron.CreateTransient(
                        authorNeuronId,
                        null,
                        null,
                        null
                    ));
                    // guest neuron
                    nn.AddReplace(Neuron.CreateTransient(
                        anonymousNeuronId,
                        null,
                        null,
                        null
                    ));
                    var tr = container.Resolve<ITransaction>();
                    await tr.BeginAsync(authorNeuronId);
                    await container.Resolve<INetworkTransactionService>().SaveAsync(tr, nn);

                    IEnumerable<object> initMirrorKeys = AvatarItemWriteRepository.GetAppMirrorKeys();
                    // initialize mirrors
                    var mr = container.Resolve<IMirrorRepository>();
                    var missingInitMirrorConfigs = await mr.GetAllMissingAsync(initMirrorKeys);
                    AssertionConcern.AssertArgumentTrue(
                        await mr.Initialize(missingInitMirrorConfigs.Select(mimc => mimc.Keys.First())),
                        "Failed initializing Mirrors."
                    );

                    // register mirrorset
                    container.Register(await mr.CreateMirrorSet());
                    container.AddWriters();
                    container.AddReaders();
                    container.Register<Id23neurULizerOptions, InProcessneurULizerOptions>();
                    container.Register<IneurULizer, neurULizer>();
                    container.Register(new Network());
                    container.Register<IAvatarWriteRepository, AvatarWriteRepository>();

                    // initialize avatar
                    await container.Resolve<IAvatarWriteRepository>().Save(
                        new Domain.Model.Avatars.Avatar()
                        {
                            Id = authorNeuronId,
                            Name = avatarItem.OwnerName,
                            CreationTimestamp = DateTimeOffset.Now
                        }
                    );

                    await tr.CommitAsync();
                }
                else
                {
                    using var defaultCommand = new SqliteCommand(sqlStatements, connection);
                    await defaultCommand.ExecuteNonQueryAsync();
                }
            }

            // Revert changes to current directory
            Directory.SetCurrentDirectory(originalDirectory);
        }

        private static IEnumerable<object> GetAppMirrorKeys() =>
            MirrorInfo.CoreMirrorsKeys
                .Concat(MirrorInfo.CoreTypes)
                .Concat(ReflectionExtensions.GetMirrorKeys(typeof(Domain.Model.Avatars.Avatar)));
    }
}
