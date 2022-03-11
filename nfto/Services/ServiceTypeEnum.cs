using Nfto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfto.Services
{
    public enum ServiceTypeEnum
    {
        ReadInline = 0,
        ReadFile = 1,
        NftOwnership = 2,
        WalletOwnership = 3,
        Reset = 4
    }

}
