using ei8.Avatar.Installer.Domain.Model.Avatars.Settings;

namespace ei8.Avatar.Installer.Domain.Model.Avatars;

// TODO:0 settings that are not part of variables.env (eg. un8y/variables.env) should be part of avatar item
public class AvatarSettings
{
    public EventSourcingSettings EventSourcing { get; set; } = new();
    public CortexGraphSettings CortexGraph { get; set; } = new();
    public AvatarApiSettings AvatarApi { get; set; } = new();
    public IdentityAccessSettings IdentityAccess { get; set; } = new();
    public CortexLibrarySettings CortexLibrary { get; set; } = new();
    public CortexDiaryNucleusSettings CortexDiaryNucleus { get; set; } = new();
    public CortexChatNucleusSettings CortexChatNucleus { get; set; } = new();
    public CortexGraphPersistenceSettings CortexGraphPersistence { get; set; } = new();
    public SshSettings Ssh { get; set; } = new();
}
