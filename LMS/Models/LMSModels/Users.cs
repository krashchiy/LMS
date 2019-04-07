using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Users
    {
        public Users()
        {
            Admins = new HashSet<Admins>();
            Professors = new HashSet<Professors>();
            Students = new HashSet<Students>();
        }

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Dob { get; set; }

        public virtual ICollection<Admins> Admins { get; set; }
        public virtual ICollection<Professors> Professors { get; set; }
        public virtual ICollection<Students> Students { get; set; }
    }
}
