using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }

        [Display(Name = "Wins")]
        public int Wins { get; set; }

        [Display(Name = "Draws")]
        public int Draws { get; set; }

        [Display(Name = "Losses")]
        public int Losses { get; set; }

        [Display(Name = "Active Game")]
        public Nullable<int> ActiveGameId { get; set; }

        public DateTime LastModifiedTime { get; set; }

        [Display(Name = "Total Points")]
        public int Points { get; set; }
    }
}
