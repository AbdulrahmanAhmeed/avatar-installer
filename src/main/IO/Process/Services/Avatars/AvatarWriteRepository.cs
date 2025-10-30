using ei8.Avatar.Installer.Domain.Model.Avatars;
using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23.neurULization.Persistence;
using ei8.Cortex.Coding.Persistence;
using ei8.EventSourcing.Client;

namespace ei8.Avatar.Installer.IO.Process.Services.Avatars
{
    public class AvatarWriteRepository : InProcessWriteRepositoryBase<Domain.Model.Avatars.Avatar>, IAvatarWriteRepository
    {
        public AvatarWriteRepository(
            ITransaction transaction,
            INetworkTransactionData networkTransactionData,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer,
            IMirrorRepository mirrorRepository
        ) : base
        (
            mirrorRepository, 
            transaction, 
            networkTransactionData, 
            networkTransactionService, 
            neurULizer
        )
        {
        }

        protected override Guid GetId(Domain.Model.Avatars.Avatar value) => value.Id;
    }
}
