using Eigakan.Domain.Models;
using Eigakan.Domain.Response.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.ContractResponse
{
    public class ContractGetAllResponse
    {
        public string Id { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? ContractDate { get; set; } // ngày kí hợp đồng trong file pdf
        public DateTime? StartDate { get; set; } // ngày duyệt phim public lên web
        public DateTime? EndDate { get; set; } // ngày hết hạn hợp đồng
        public int? Duration { get; set; } //thời gian hợp đồng
        public decimal? Price { get; set; } //giá hợp đồng
        public string? Terms { get; set; } //chính sách
        public string? PublisherName { get; set; }
        public string? DistributorName { get; set; }
        public DateTime? CreateDate { get; set; } //ngày tạo bảng
        public DateTime? UpdateDate { get; set; } //ngày cập nhật bảng
        public string? Status { get; set; }
        public string? ReasonForDenying { get; set; }

        public UserGetAllResponse? User { get; set; }

        public MovieGetAllResponse Movie { get; set; }
    }
}

