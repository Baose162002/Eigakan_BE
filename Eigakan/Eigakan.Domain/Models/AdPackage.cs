using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class AdPackage
	{
		public string? Id { get; set; } 
		public string? PackageName { get; set; }
        public int? MinView { get; set; }
		public int? MaxView { get; set; }
        public decimal? PricePerView { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }

	}
}
