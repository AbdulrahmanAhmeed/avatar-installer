using ei8.Avatar.Installer.Common;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.Domain.Model.Avatars.Settings;
using Microsoft.Extensions.Logging;

namespace ei8.Avatar.Installer.IO.Process.Services.Avatars
{
    public class AvatarItemReadRepository : IAvatarItemReadRepository
    {
        private readonly ILogger<AvatarItemWriteRepository> logger;

        public AvatarItemReadRepository(
            ILogger<AvatarItemWriteRepository> logger
        )
        {
            this.logger = logger;
        }

        // TODO: Add unit tests
        public async Task<AvatarItem> GetByAsync(string id)
        {
            if (!Directory.Exists(id))
            {
                logger.LogInformation("No files found in {id}", id);
                return null;
            }

            // assuming id is a path string
            var avatarItem = new AvatarItem(id);

            foreach (var file in Directory.EnumerateFiles(id))
            {
                switch (Path.GetFileName(file))
                {
                    // load other files here as needed
                    case Common.Constants.Filenames.VariablesEnv:
                        logger.LogInformation("Loading {file}", file);

                        var variables = await GetEnvironmentVariablesFromFileAsync(file);
                        avatarItem.Settings.CortexGraph = DeserializeEnvironmentVariables<CortexGraphSettings>(variables);
                        avatarItem.Settings.EventSourcing = DeserializeEnvironmentVariables<EventSourcingSettings>(variables);
                        avatarItem.Settings.AvatarApi = DeserializeEnvironmentVariables<AvatarApiSettings>(variables);
                        avatarItem.Settings.IdentityAccess = DeserializeEnvironmentVariables<IdentityAccessSettings>(variables);
                        avatarItem.Settings.CortexLibrary = DeserializeEnvironmentVariables<CortexLibrarySettings>(variables);
                        avatarItem.Settings.CortexDiaryNucleus = DeserializeEnvironmentVariables<CortexDiaryNucleusSettings>(variables);
                        avatarItem.Settings.CortexChatNucleus = DeserializeEnvironmentVariables<CortexChatNucleusSettings>(variables);
                        break;

                    case Common.Constants.Filenames.Env:
                        logger.LogInformation("Loading {file}", file);

                        avatarItem.OrchestrationSettings = DeserializeEnvironmentVariables<OrchestrationSettings>(
                            await GetEnvironmentVariablesFromFileAsync(file)
                        );
                        break;
                    case Common.Constants.Filenames.SshConfig:
                        logger.LogInformation("Loading {file}", file);

                        avatarItem.Settings.Ssh = await DeserializeSshSettingsFile(Path.Combine(id, Constants.Filenames.SshConfig), logger);
                        break;
                }
            }

            // un8y
            foreach (var file in Directory.EnumerateFiles(id + "/" + Common.Constants.Directories.Un8y))
            {
                switch (Path.GetFileName(file))
                {
                    case Common.Constants.Filenames.VariablesEnv:
                        logger.LogInformation("Loading {file}", file);

                        var variables = await GetEnvironmentVariablesFromFileAsync(file);
                        avatarItem.Un8ySettings = DeserializeEnvironmentVariables<Un8ySettings>(variables);
                        break;
                }
            }


            return avatarItem;
        }

        private static async Task<Dictionary<string, string>> GetEnvironmentVariablesFromFileAsync(string file)
        {
            return (await File.ReadAllLinesAsync(file))
                              .Where(l => !l.StartsWith('#') && !string.IsNullOrWhiteSpace(l)) // ignore comments and newlines
                              .Select(l => l.Split('='))                                       // split into variable, value
                              .ToDictionary(l => l[0], l => l[1]);
        }

        private T DeserializeEnvironmentVariables<T>(Dictionary<string, string> variables)
            where T : class, new()
        {
            if (!variables.Any())
                return null;

            var settings = new T();

            foreach (var property in settings.GetType().GetProperties())
            {
                var environmentVariableKey = property.Name.ToMacroCase();

                if (variables.TryGetValue(environmentVariableKey, out var savedValue))
                    property.SetValueFromString(settings, savedValue);
            }

            return settings;
        }

        private static async Task<SshSettings> DeserializeSshSettingsFile(string fileName, ILogger<AvatarItemWriteRepository> logger)
        {
            if (!File.Exists(fileName))
            {
                logger.LogWarning("Unable to find SSH settings file: {fileName}", fileName);
                return null;
            }

            logger.LogInformation("Deserializing {fileName}", fileName);

            SshSettings result = null;

            using (var file = new StreamReader(fileName))
            {
                string line;
                string currentKey = null;

                while ((line = await file.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("Host"))
                    {
                        var hostLine = line.Split(' ');
                        currentKey = hostLine[1];

                        result = new SshSettings();
                    }
                    else if (line.StartsWith('\t') || line.StartsWith("    "))
                    {
                        var settingLine = line.TrimStart()
                                              .Split(' ');

                        var propName = settingLine[0];
                        var propValue = string.Join(' ', settingLine.Skip(1));

                        var prop = typeof(SshSettings).GetProperty(propName);

                        if (prop != null)
                            prop.SetValueFromString(result, propValue);
                    }
                }
            }

            return result;
        }
    }
}
