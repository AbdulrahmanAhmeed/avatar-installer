using AutoMapper;
using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Avatar.Installer.Domain.Model.Avatars.Settings;
using ei8.Avatar.Installer.Domain.Model.Configuration;

namespace ei8.Avatar.Installer.Domain.Model.Mapping
{
    public class AvatarMapperService : IAvatarMapperService
    {
        private readonly IMapper mapper;

        public AvatarMapperService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AvatarItem Apply(AvatarConfigurationItem configItem, AvatarItem avatarItem)
        {
            return mapper.Map(configItem, avatarItem);
        }
    }

    public class AvatarAutoMapperProfile : Profile
    {
        public AvatarAutoMapperProfile()
        {
            CreateMap<AvatarConfigurationItem, AvatarItem>()
                .ForPath(
                    dest => dest.RoutingSettings,
                    opt => opt.MapFrom(src => src.Routing)
                )
                .ForPath(
                    dest => dest.Settings.CortexGraphPersistence,
                    opt => opt.MapFrom(src => src.CortexGraphPersistence)
                )
                .ForPath(
                    dest => dest.Settings.CortexGraph,
                    opt => opt.MapFrom(src => src.CortexGraph)
                )
                .ForPath(
                    dest => dest.Settings.AvatarApi,
                    opt => opt.MapFrom(src => src.AvatarApi)
                )
                .ForPath(
                    dest => dest.Settings.CortexLibrary,
                    opt => opt.MapFrom(src => src.CortexLibrary)
                )
                .ForPath(
                    dest => dest.OrchestrationSettings,
                    opt => opt.MapFrom(src => src.Orchestration)
                )
                .ForPath(
                    dest => dest.Settings.CortexChatNucleus,
                    opt => opt.MapFrom(src => src.CortexChatNucleus)
                )
                .ForPath(
                    dest => dest.Un8ySettings,
                    opt => opt.MapFrom(src => src.Un8y)
                )
                .ForPath(
                    dest => dest.Settings.EventSourcing,
                    opt => opt.MapFrom(src => src.EventSourcing)
                )
                .ForPath(
                    dest => dest.Settings.Ssh,
                    opt => opt.MapFrom(src => src.Ssh)
                )
                .AfterMap((s, d) =>
                {
                    d.Settings.Ssh.RemoteForward = $"{s.Orchestration.AvatarName}:80 localhost:{s.Orchestration.TunnelLocalPort}";
                })
                .AfterMap((s, d) =>
                {
                    d.Settings.CortexChatNucleus.AppUserId = s.OwnerUserId;
                });

            CreateMap<RoutingConfiguration, RoutingSettings>();
            CreateMap<CortexGraphPersistenceConfiguration, CortexGraphPersistenceSettings>();
            CreateMap<CortexGraphConfiguration, CortexGraphSettings>();
            CreateMap<AvatarApiConfiguration, AvatarApiSettings>();
            CreateMap<CortexLibraryConfiguration, CortexLibrarySettings>();
            CreateMap<OrchestrationConfiguration, OrchestrationSettings>();
            CreateMap<CortexChatNucleusConfiguration, CortexChatNucleusSettings>();
            CreateMap<Un8yConfiguration, Un8ySettings>()
                .ForMember(
                    dest => dest.OidcAuthority,
                    opt => opt.MapFrom(src => src.OidcAuthorityUrl)
                );
            CreateMap<EventSourcingConfiguration, EventSourcingSettings>();
            CreateMap<SshConfiguration, SshSettings>();
        }
    }
}
