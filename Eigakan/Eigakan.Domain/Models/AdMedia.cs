using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class AdMedia
	{
        public string? Id { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public string? ReasonForRejection { get; set; }
        public string? status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public ICollection<AdPurchaseItems>? AdPurchaseItems { get; set; }
        public ICollection<AdMediaCount>? adMediaCounts { get; set; }

    }
}
