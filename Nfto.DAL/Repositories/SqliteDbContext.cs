using Nfto.DAL.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfto.DAL.Repositories
{
 
    /// <summary>
    /// Persistence of Data through SQLite.
    /// </summary>
    public class SqliteDbContext : IDbContext
    {

        SQLiteAsyncConnection db;
        public SqliteDbContext(string databasePath)
        {

            db = new SQLiteAsyncConnection(databasePath);

        }

        public async Task<bool> DeleteTokenAsync(string tokenId)
        {
            try
            {
                await createDatabaseIfNotExists();

                int nbRows = await db.Table<Nft>().DeleteAsync(p => p.TokenId.Equals(tokenId));

                return nbRows == 1;
            }catch
            {
                throw;
            }
         
        }

        private async Task createDatabaseIfNotExists()
        {
            await db.CreateTableAsync<Nft>();
        }

        public async Task<List<Nft>> GetAllTokenForAddressAsync(string address)
        {
            try
            {
                await createDatabaseIfNotExists();

                var query = db.Table<Nft>().Where(n => n.Address.Equals(address));

                var result = await query.ToListAsync();

                return result;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<Nft> GetNftAsync(string tokenId)
        {
            try
            {
                await createDatabaseIfNotExists();

                var nft = await db.Table<Nft>().FirstOrDefaultAsync(n => n.TokenId == tokenId);

                return nft;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> ResetDatabaseAsync()
        {
            try
            {
                await createDatabaseIfNotExists();

                int nbRows = await db.DeleteAllAsync<Nft>();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SaveMintAsync(Nft model)
        {
            try
            {
                await createDatabaseIfNotExists();

                if (await GetNftAsync(model.TokenId) != null)
                {
                    Console.WriteLine("Item already exists");
                    return false;
                }
                Console.WriteLine($"Saving {model}");
                int nbRows = await db.InsertAsync(model);

                return (nbRows == 1);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> TransferTokenAsync(string tokenId, string from, string to)
        {
            try
            {
                await createDatabaseIfNotExists();

                var nft = await GetNftAsync(tokenId);
                if (nft == null)
                {
                    Console.WriteLine($"Nft with token {tokenId} does not exist");
                    return false;
                }

                if (!nft.Address.Equals(from))
                {
                    Console.WriteLine($"Nft with token {tokenId} does not belong to {from}");
                    return false;
                }

                nft.Address = to;

                int nbRows = await db.UpdateAsync(nft);

                return (nbRows == 1);

            }
            catch 
            {
                throw;
            }
        }
    }
}
