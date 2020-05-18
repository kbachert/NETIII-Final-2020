using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class User
    {
        public int EmployeeID { get; set; }

        [Required]
        [MaxLength(35, ErrorMessage = "First Name can be no longer than 35 characters!")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(35, ErrorMessage = "Last Name can be no longer than 35 characters!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [MinLength(7, ErrorMessage = "Email Address can be no less than 7 characters!")]
        [MaxLength(250, ErrorMessage = "Email Address can be no longer than 250 characters!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Range(0, Int64.MaxValue, ErrorMessage = "Phone number should not contain non-numeric characters")]
        [MinLength(7, ErrorMessage = "Phone Number can be no less than 7 characters!")]
        [MaxLength(11, ErrorMessage = "Phone Number can be no longer than 11 characters!")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public bool Active { get; set; }

        public List<string> Roles { get; set; }
    }
}
