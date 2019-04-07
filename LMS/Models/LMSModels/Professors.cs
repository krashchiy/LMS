using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public string ProfessorId { get; set; }
        public int DeptId { get; set; }

        public virtual Departments Dept { get; set; }
        public virtual Users Professor { get; set; }
    }
}
