using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public string StudentId { get; set; }
        public int DeptId { get; set; }

        public virtual Departments Dept { get; set; }
        public virtual Users Student { get; set; }
    }
}
