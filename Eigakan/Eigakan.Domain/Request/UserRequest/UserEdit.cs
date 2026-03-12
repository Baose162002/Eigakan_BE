using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.UserRequest
{
    public class UserEdit
    {
        [Required]

        public string? FullName { get; set; }
        [Required]

        public bool? Gender { get; set; }
        [Required]
        [DataType(DataType.Date)]  // Ensures it’s a date (yyyy-mm-dd format)
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Invalid date format. Please use yyyy-mm-dd.")]
        public string? Birthday { get; set; }

        [Required]

        public string? Picture { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Invalid email format. Please provide a valid Gmail address.")]

        public string? Email { get; set; }
            
        
    }
}
