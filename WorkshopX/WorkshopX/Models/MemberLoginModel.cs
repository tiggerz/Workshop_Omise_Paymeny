using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopX.Models
{
    public class MemberLoginModel
    {
        public int MemberId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CitizenId { get; set; }
        public string MobileNo { get; set; }
        public string DateOfBirth { get; set; }

        //Make Date

        //public int MyProperty { get; set; }
    }
}
