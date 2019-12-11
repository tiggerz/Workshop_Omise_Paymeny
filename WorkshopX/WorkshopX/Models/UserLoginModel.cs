using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkshopX.Models
{
    class UserLoginModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        //[Required]
        public string Password { get; set; }
    }
}
