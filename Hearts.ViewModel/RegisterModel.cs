using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class RegisterModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(6)]
        [StringLength(20)]
        [RegularExpression(
            "^[a-zA-Z0-9]{6,20}$", 
            ErrorMessage = "Enter a valid aplha-numeric name. 6-20 Characters.", 
            MatchTimeoutInMilliseconds = 5)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid Email Id.")]
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }

        [Required]
        [MinLength(8)]
        [StringLength(20)]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$", 
            ErrorMessage = "Must include atleast one Uppercase letter, Lowercase letter, Number and Special Character. 8 - 20 Characters.",
            MatchTimeoutInMilliseconds = 5)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string hashRandomSeed { get; set; }

        public bool isSuccess { get; set; }
    }
}
