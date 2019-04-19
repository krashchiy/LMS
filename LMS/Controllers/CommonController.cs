using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/
        protected Team45LMSContext db;

        public CommonController()
        {
            db = new Team45LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */
        public void UseLMSContext(Team45LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            var result = from d in db.Departments select new { name = d.Name, subject = d.Abbrev };
            return Json(result.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var result = from d in db.Departments
                join cr in db.Courses on d.DeptId equals cr.DeptId
                select new
                {
                    subject = d.Abbrev,
                    dname = d.Name,
                    courses = new
                    {
                        number = cr.Number,
                        cname = cr.Name
                    }
                };
            return Json(result.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {

            var result =
                  from c in db.Classes
                  join crs in db.Courses on c.CatalogId equals crs.CatalogId
                  join d in db.Departments on crs.DeptId equals d.DeptId
                  join u in db.Users on c.ProfessorId equals u.UserId
                  where d.Abbrev == subject && Convert.ToInt32(crs.Number) == number
                  select new
                  {
                      season = c.Semester,
                      year = c.Year,
                      location = c.Location,
                      start = c.StartTime,
                      end = c.EndTime,
                      fname = u.FirstName,
                      lname = u.LastName
                  };

            return Json(result);
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            var result = from a in db.Assignments
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season 
                      && c.Year == year && ac.Name == category && a.Name == asgname
                select a.Contents;
            return Content(result.FirstOrDefault());
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            //Make sure to get the latest submission in case there are many
            var result = from sm in db.Submissions
                join a in db.Assignments on sm.AsgId equals a.AsgId
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season 
                      && c.Year == year && ac.Name == category && a.Name == asgname && sm.StudentId == uid
                orderby sm.TimeSubmitted descending 
                select sm.Contents;

            return Content(result.FirstOrDefault());
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            //check to see if the user id exists.  If not, return no success.
            var id = (from u in db.Users where u.UserId == uid select u.UserId).FirstOrDefault();
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false });

            //Check if the Uid belongs to student, if so, return student info.
            var proposedStudent = (from s in db.Students where s.StudentId == uid select s.StudentId).FirstOrDefault();
            if (!string.IsNullOrEmpty(proposedStudent))
            {

                var studentResult =
                      from u in db.Users
                      join s in db.Students on u.UserId equals s.StudentId
                      join d in db.Departments on s.DeptId equals d.DeptId
                      where u.UserId == uid
                      select new
                      {
                          fname = u.FirstName,
                          lname = u.LastName,
                          uid = u.UserId,
                          department = d.Name
                      };

                return Json(studentResult);
            }

            //check to see if uID belongs to Professor, if so, return professor info.
            var proposedProfessor = (from p in db.Professors where p.ProfessorId == uid select p.ProfessorId).FirstOrDefault();
            if (!string.IsNullOrEmpty(proposedProfessor))
            {

                var professorResult =
                      from u in db.Users
                      join p in db.Professors on u.UserId equals p.ProfessorId
                      join d in db.Departments on p.DeptId equals d.DeptId
                      where u.UserId == uid
                      select new
                      {
                          fname = u.FirstName,
                          lname = u.LastName,
                          uid = u.UserId,
                          department = d.Name
                      };

                return Json(professorResult);
            }

            //check to see if uID belongs to Professor, if so, return professor info.
            var proposedAdmin = (from a in db.Admins where a.AdminId == uid select a.AdminId).FirstOrDefault();
            if (!string.IsNullOrEmpty(proposedAdmin))
            {

                var adminResult =
                      from u in db.Users
                      join a in db.Admins on u.UserId equals a.AdminId
                      where u.UserId == uid
                      select new
                      {
                          fname = u.FirstName,
                          lname = u.LastName,
                          uid = u.UserId,
                      };

                return Json(adminResult);
            }
            return Json(new { success = false });
        }
        /*******End code to modify********/

    }
}