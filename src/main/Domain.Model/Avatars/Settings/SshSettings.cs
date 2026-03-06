namespace ei8.Avatar.Installer.Domain.Model.Avatars.Settings
{
    public class SshSettings
    {
        public int ServerAliveInterval { get; set; }
        public int ServerAliveCountMax { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public string RemoteForward { get; set; }
    }
}
