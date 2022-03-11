namespace Nfto.Models
{
    public class MintMessage : MessageBase
    {
        /// <summary>
        /// The id of the token
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// The address of the token
        /// </summary>
        public string Address { get; set; }

        public override string ToString()
        {
            return $"Mint: Token={TokenId} Address={Address}";
        }

    }
}
