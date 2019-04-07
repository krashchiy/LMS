using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrollment
    {
        public string StudentId { get; set; }
        public int ClassId { get; set; }
        public string Grade { get; set; }

        public virtual Classes Class { get; set; }
    }
}
