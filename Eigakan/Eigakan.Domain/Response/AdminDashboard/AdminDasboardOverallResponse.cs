using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdminDashboard
{
	public class AdminDasboardOverallResponse
	{
		public int TotalUsers { get; set; }
		public int ActiveUsers { get; set; }

		public int TotalMovies { get; set; }
		public int ActiveMovies { get; set; }

		public int TotalUserRegisters { get; set; }
		public int AcceptedUserRegisters { get; set; }

		public int TotalContracts { get; set; }
		public int SignedContracts { get; set; }
	}
}
