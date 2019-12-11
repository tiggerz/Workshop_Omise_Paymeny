using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopX.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
