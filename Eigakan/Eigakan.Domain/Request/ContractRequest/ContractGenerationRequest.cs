using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.ContractRequest
{
    public class ContractGenerationRequest
    {
        public string? StartDate { get; set; }
        public int Duration { get; set; } 
        public decimal? Price { get; set; }
        public string? PublisherName { get; set; }
        public string? DistributorName { get; set; }
        public string? MovieId { get; set; }

        public DateTime? GetStartDate() => ParseDate(StartDate);

        private DateTime? ParseDate(string? dateString)
        {
            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null;
        }
    }

}
