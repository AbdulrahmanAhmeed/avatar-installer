namespace ei8.Avatar.Installer.Domain.Model.Plugins
{
    public interface IPluginsService
    {
        /// <summary>
        /// Download a ZIP archive from <paramref name="pluginsUrl"/> and extract its contents into <paramref name="destinationPath"/>.
        /// </summary>
        Task DownloadAndExtractAsync(string destinationPath, string pluginsUrl);
    }
}
