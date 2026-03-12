using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdMediaResponse
{
    public class AdMediaWithPositionDto
    {
        public string AdMediaId { get; set; }
        public int Position { get; set; }
        public AdMediaGetAll AdMedia { get; set; } // Thêm toàn bộ dữ liệu của AdMedia
    }
    public class AdMediaGetAll
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
