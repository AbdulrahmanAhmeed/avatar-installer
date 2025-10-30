using ei8.Cortex.Coding.d23.neurULization;

namespace ei8.Avatar.Installer.IO.Process.Services.Avatars
{
    internal readonly struct MirrorInfo
    {
        internal static readonly IEnumerable<object> CoreMirrorsKeys = typeof(MirrorSet).GetProperties().Select(p => p.Name).Cast<object>();
        internal static readonly Type[] CoreTypes = new Type[] {
            typeof(string),
            typeof(Guid),
            typeof(DateTimeOffset)
        };
    }
}
