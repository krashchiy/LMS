using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public string CatalogId { get; set; }
        public int? DeptId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public virtual Departments Dept { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
