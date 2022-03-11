using Nfto.DAL.Models;
using Nfto.Models;

namespace Nfto.Services
{
    public interface INftoService
    {
        Task<Result<MessageBase[]>> RunReadLineAsync(string jsonParameter);
        Task<Result<MessageBase[]>> RunReadFileAsync(string filePath);

        Task<Result<Nft>> RunNftOwnershipAsync(string tokenId);
        Task<Result<List<Nft>>> RunWalletOwnershipAsync(string address);

        Task<Result> ResetAsync();

        void Dispose();
    }

}
