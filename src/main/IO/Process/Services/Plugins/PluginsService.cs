using ei8.Avatar.Installer.Domain.Model.Plugins;
using Microsoft.Extensions.Logging;
using neurUL.Common.Domain.Model;
using System.IO.Compression;
using System.Web;

namespace ei8.Avatar.Installer.IO.Process.Services.Plugins
{
    public class PluginsService : IPluginsService
    {
        private readonly ILogger<PluginsService> logger;

        public PluginsService(ILogger<PluginsService> logger)
        {
            this.logger = logger;
        }

        public async Task DownloadAndExtractAsync(string destinationPath, string pluginsUrl)
        {
            AssertionConcern.AssertArgumentNotNull(destinationPath, nameof(destinationPath));
            AssertionConcern.AssertArgumentNotNull(pluginsUrl, nameof(pluginsUrl));

            pluginsUrl = ResolveUrl(pluginsUrl.Trim());

            logger.LogInformation("Resolved plugins URL: '{pluginsUrl}'", pluginsUrl);

            if (TryParseGitHubTreeUrl(pluginsUrl, out var owner, out var repo, out var branch, out var subPath))
                await DownloadGitHubDirectoryAsync(destinationPath, owner, repo, branch, subPath);
            else
                await DownloadZipAsync(destinationPath, pluginsUrl);
        }

        /// <summary>
        /// Unwraps download-directory.github.io URLs by extracting the embedded GitHub URL
        /// from the <c>url</c> query parameter. Other URLs are returned as-is.
        /// </summary>
        private static string ResolveUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return url;

            if (uri.Host.Equals("download-directory.github.io", StringComparison.OrdinalIgnoreCase))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                var embedded = query["url"];
                if (!string.IsNullOrWhiteSpace(embedded))
                    return embedded;
            }

            return url;
        }

        private static bool TryParseGitHubTreeUrl(
            string url,
            out string owner,
            out string repo,
            out string branch,
            out string subPath)
        {
            owner = repo = branch = subPath = null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                return false;

            // Expected path: /{owner}/{repo}/tree/{branch}[/{subPath}]
            var segments = uri.AbsolutePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 4 || !segments[2].Equals("tree", StringComparison.OrdinalIgnoreCase))
                return false;

            owner = segments[0];
            repo = segments[1];
            branch = segments[3];
            subPath = segments.Length > 4
                ? string.Join("/", segments.Skip(4))
                : string.Empty;

            return true;
        }

        private async Task DownloadGitHubDirectoryAsync(
            string destinationPath, string owner, string repo, string branch, string subPath)
        {
            var archiveUrl = $"https://github.com/{owner}/{repo}/archive/refs/heads/{branch}.zip";

            logger.LogInformation(
                "Detected GitHub directory URL. Downloading archive from {archiveUrl}...",
                archiveUrl
            );

            var tempZipPath = Path.GetTempFileName();
            var tempExtractDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AvatarInstaller/1.0");
                    using var response = await httpClient.GetAsync(archiveUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fileStream);
                }

                logger.LogInformation("Extracting archive...");
                ZipFile.ExtractToDirectory(tempZipPath, tempExtractDir);

                // GitHub archives have a root folder named "{repo}-{branch}/"
                var rootDir = Directory.GetDirectories(tempExtractDir).FirstOrDefault();
                if (rootDir == null)
                    throw new InvalidOperationException("Downloaded archive is empty.");

                var sourcePath = string.IsNullOrEmpty(subPath)
                    ? rootDir
                    : Path.Combine(rootDir, subPath.Replace('/', Path.DirectorySeparatorChar));

                if (!Directory.Exists(sourcePath))
                    throw new DirectoryNotFoundException(
                        $"Path '{subPath}' not found in the downloaded archive."
                    );

                logger.LogInformation("Copying plugins to {destinationPath}...", destinationPath);
                CopyDirectory(sourcePath, destinationPath);

                logger.LogInformation("Plugins extracted successfully to {destinationPath}.", destinationPath);
            }
            finally
            {
                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);
                if (Directory.Exists(tempExtractDir))
                    Directory.Delete(tempExtractDir, recursive: true);
            }
        }

        private async Task DownloadZipAsync(string destinationPath, string pluginsUrl)
        {
            var tempZipPath = Path.GetTempFileName();

            try
            {
                logger.LogInformation("Downloading ZIP from {pluginsUrl}...", pluginsUrl);

                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(pluginsUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fileStream);
                }

                logger.LogInformation("Extracting plugins to {destinationPath}...", destinationPath);

                Directory.CreateDirectory(destinationPath);
                ZipFile.ExtractToDirectory(tempZipPath, destinationPath, overwriteFiles: true);

                logger.LogInformation("Plugins extracted successfully to {destinationPath}.", destinationPath);
            }
            finally
            {
                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}
