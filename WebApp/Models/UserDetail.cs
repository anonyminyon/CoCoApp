using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class UserDetail
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime Dob { get; set; }
        public bool Gender { get; set; }

        public virtual User? User { get; set; }
    }
}
