using ei8.Avatar.Installer.Domain.Model.Avatars.Settings;
using neurUL.Common.Domain.Model;

namespace ei8.Avatar.Installer.Domain.Model.Avatars
{
    /// <summary>
    /// A concrete representation of an avatar, derived from a <see cref="Configuration.AvatarServerConfiguration"/>
    /// </summary>
    public class AvatarItem
    {
        public string Id { get; set; }

        /// <summary>
        /// The name of the neurULized Avatar instance.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The user id which is mapped to the neurULized Avatar instance in Iden8y.
        /// </summary>
        public string OwnerUserId { get; set; }

        public RoutingSettings RoutingSettings { get; set; } = new();
        public AvatarSettings Settings { get; set; } = new();
        public Un8ySettings Un8ySettings { get; set; } = new();
        public OrchestrationSettings OrchestrationSettings { get; set; } = new();

        public AvatarItem(string id)
        {
            AssertionConcern.AssertArgumentNotEmpty(id, "Specified 'id' cannot be empty.", nameof(id));
            AssertionConcern.AssertArgumentNotNull(id, nameof(id));

            this.Id = id;
        }
    }
}
