using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Admins
    {
        public string AdminId { get; set; }
        public int DeptId { get; set; }

        public virtual Users Admin { get; set; }
        public virtual Departments Dept { get; set; }
    }
}
