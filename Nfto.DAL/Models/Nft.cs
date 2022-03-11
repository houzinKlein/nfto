using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfto.DAL.Models
{
    [Table("Nfts")]
    public class Nft
    {

        [PrimaryKey]
        [Column("tokenId")]
        public string TokenId { get; set; }

        [Indexed]
        [Column("address")]
        public string Address { get; set; }

        public Nft()
        {

        }

        public Nft(string tokenId, string address)
        {
            TokenId = tokenId;
            Address = address;
        }

        public override string ToString()
        {
            return $"TokenId={TokenId} Address={Address}";
        }
    }

}
