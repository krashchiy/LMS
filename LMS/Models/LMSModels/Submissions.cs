using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public int SubmissionId { get; set; }
        public string StudentId { get; set; }
        public int? AsgId { get; set; }
        public string Contents { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public int? Score { get; set; }

        public virtual Assignments Asg { get; set; }
    }
}
