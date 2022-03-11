namespace Nfto.Models
{
    /// <summary>
    /// The Burn message model
    /// </summary>
    public class BurnMessage : MessageBase
    {
        /// <summary>
        /// The token to destroy
        /// </summary>
        public string TokenId { get; set; }

        public override string ToString()
        {
            return $"Burn: Token={TokenId}";
        }

    }
}
