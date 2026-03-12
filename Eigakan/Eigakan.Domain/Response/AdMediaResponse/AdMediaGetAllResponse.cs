using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdMediaResponse
{
    public class AdMediaGetAllResponse
    {
        public string? Id { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public string? ReasonForRejection { get; set; }
        public string? status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
