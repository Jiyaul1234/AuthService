using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.AuthService.Domain.Models
{
    [Table("User")]
    public  class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public string Role { get; set; } = "User";

        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
