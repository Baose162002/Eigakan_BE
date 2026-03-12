using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.ContractRequest
{
	public class AcceptContractRequest
	{
        public string? Id { get; set; }
        public string? SignToken { get; set; }
    }
}
