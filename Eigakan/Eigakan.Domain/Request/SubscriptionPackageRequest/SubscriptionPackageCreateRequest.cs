using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.SubscriptionPackageRequest
{
    public class SubscriptionPackageCreateRequest
    {
        public string? PackageName { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }

    }
}
