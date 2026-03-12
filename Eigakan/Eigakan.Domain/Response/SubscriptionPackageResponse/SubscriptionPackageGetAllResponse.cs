using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.SubscriptionPackageResponse
{
    public class SubscriptionPackageGetAllResponse
    {
        public string Id { get; set; }
        public string? PackageName { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Status { get; set; }

    }
}
