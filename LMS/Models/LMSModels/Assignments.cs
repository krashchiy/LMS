using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public int AsgId { get; set; }
        public int AsgCatId { get; set; }
        public string Name { get; set; }
        public int? MaxPoints { get; set; }
        public string Contents { get; set; }
        public DateTime? DueDate { get; set; }

        public virtual AssignmentCategories AsgCat { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
