using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Departments
    {
        public Departments()
        {
            Admins = new HashSet<Admins>();
            Courses = new HashSet<Courses>();
            Professors = new HashSet<Professors>();
            Students = new HashSet<Students>();
        }

        public int DeptId { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }

        public virtual ICollection<Admins> Admins { get; set; }
        public virtual ICollection<Courses> Courses { get; set; }
        public virtual ICollection<Professors> Professors { get; set; }
        public virtual ICollection<Students> Students { get; set; }
    }
}
