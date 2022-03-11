namespace Nfto.Models
{
    /// <summary>
    /// The transfer message model
    /// </summary>
    public class TransferMessage : MessageBase
    {
        /// <summary>
        /// The token Id to transfer
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// The address to transfer from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The Address to transfer to
        /// </summary>
        public string To { get; set; }

        public override string ToString()
        {
            return $"Mint: Token={TokenId} From={From} To={To}";
        }

    }
}
