using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPackage
{
    public class AdPackageCreateRequest
    {
        [Required(ErrorMessage = "Package name is required.")] 
        public string? PackageName { get; set; }
		[Range(20, int.MaxValue, ErrorMessage = "MinView must be at least 20")]
		public int? MinView { get; set; }
		[Range(0, 5000, ErrorMessage = "MaxView must be at most 5000")]
		public int? MaxView { get; set; }
		[Range(0.01, double.MaxValue, ErrorMessage = "Price per view must be greater than 0")]
		public decimal? PricePerView { get; set; }

	}


}

