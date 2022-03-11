using Nfto.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nfto.DAL.Repositories
{
    public interface IDbContext
    {
        Task<bool> DeleteTokenAsync(string tokenId);
        Task<List<Nft>> GetAllTokenForAddressAsync(string address);
        Task<Nft> GetNftAsync(string tokenId);
        Task<bool> ResetDatabaseAsync();
        Task<bool> SaveMintAsync(Nft model);
        Task<bool> TransferTokenAsync(string tokenId, string from, string to);
    }
}