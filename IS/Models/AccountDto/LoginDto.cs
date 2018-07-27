using System.ComponentModel.DataAnnotations;

namespace IS.Models.AccountDto
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
