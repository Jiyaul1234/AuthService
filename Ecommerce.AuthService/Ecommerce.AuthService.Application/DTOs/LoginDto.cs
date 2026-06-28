using System.ComponentModel.DataAnnotations;

namespace Ecommerce.AuthService.Application.DTOs
{
    public  class LoginDto
    {
        [Required(ErrorMessage ="Please Enter Email")]
        [EmailAddress(ErrorMessage ="Please Enter Valid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Please Enter Password")]
        public string Password { get; set; }

    }
}
