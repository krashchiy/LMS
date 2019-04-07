using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrollment = new HashSet<Enrollment>();
        }

        public int ClassId { get; set; }
        public string CatalogId { get; set; }
        public string ProfessorId { get; set; }
        public string Semester { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Location { get; set; }

        public virtual Courses Catalog { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
