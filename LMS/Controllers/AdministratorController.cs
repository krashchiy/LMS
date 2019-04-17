using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Models.AccountViewModels;
using LMS.Services;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var result =
                from d in db.Departments
                join c in db.Courses on d.DeptId equals c.DeptId
                where d.Abbrev == subject
                select new { number = c.Number, name = c.Name };

            return Json(result);
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {

            var result =
                      from d in db.Departments
                      join p in db.Professors on d.DeptId equals p.DeptId
                      join u in db.Users on p.ProfessorId equals u.UserId
                      where d.Abbrev == subject
                      select new { lname = u.LastName, fname = u.FirstName, uid = u.UserId };

            return Json(result);
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            var tableCount = db.Courses.Count();

            var departmentId = (from d in db.Departments where d.Abbrev == subject select d.DeptId).FirstOrDefault();

            //handle the case of an empty table
            if (tableCount == 0)
            {
                var newCourse = new Courses
                {
                    CatalogId = "1",
                    DeptId = departmentId,
                    Name = name,
                    Number = number.ToString()

                };

                db.Courses.Add(newCourse);
                db.SaveChanges();
                return Json(new { success = true });
            }
            //otherwise, if not empty
            else
            {
                //check to see if course is a duplicate
                string proposedCourse = (from c in db.Courses
                                         join d in db.Departments on c.DeptId equals d.DeptId
                                         where d.Abbrev == subject && c.Number == number.ToString()
                                         select c.Name).FirstOrDefault();

                //not a duplicate
                if (string.IsNullOrEmpty(proposedCourse))
                {
                    var catID = Convert.ToInt32((from c in db.Courses select c).OrderByDescending(x => x.CatalogId).Select(u => u.CatalogId).FirstOrDefault());
                    catID++;

                    var newCourse = new Courses
                    {
                        CatalogId = catID.ToString(),
                        DeptId = departmentId,
                        Name = name,
                        Number = number.ToString()
                    };

                    db.Courses.Add(newCourse);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            var tableCount = db.Classes.Count();

            var catID = (from cat in db.Courses join d in db.Departments on cat.DeptId equals d.DeptId
                         where d.Abbrev == subject && Convert.ToInt32(cat.Number) == number select cat.CatalogId).FirstOrDefault();

            //If the table is empty
            if (tableCount == 0)
            {
                var firstClass = new Classes
                {
                    ClassId = 1,
                    CatalogId = catID,
                    ProfessorId = instructor,
                    Semester = season,
                    StartTime = start,
                    EndTime = end,
                    Location = location
                };

                db.Classes.Add(firstClass);
                db.SaveChanges();
                return Json(new { success = true });
            }

            //check to see if class is a duplicate
            string possibleDuplicate = (from c in db.Classes
                                    join cat in db.Courses on c.CatalogId equals cat.CatalogId
                                    where c.Semester == season && c.CatalogId == catID
                                    select c.Location).FirstOrDefault();

            //If duplicate
            if (!string.IsNullOrEmpty(possibleDuplicate))
                return Json(new { success = false });


            //check to see if proposedClass has time conflicting with another class startTime or EndTime during the same semester
            var possibleTimeConflict = (from c in db.Classes
                                 where c.Semester == season && c.Location == location &&
                                 ( (start >= c.StartTime && start <= c.EndTime)
                                 || (end >= c.StartTime && end <= c.EndTime)
                                 || (c.StartTime >= start && c.StartTime <= end)
                                 || (c.EndTime >= start && c.EndTime <= end)
                                 ) select c.CatalogId).FirstOrDefault();

            //If conflict found
            if (!string.IsNullOrEmpty(possibleTimeConflict))
                return Json(new { success = false });

            //retrieve highest ClassId and increment by 1
            var classId = Convert.ToInt32((from c in db.Classes select c).OrderByDescending(x => x.ClassId).Select(u => u.ClassId).FirstOrDefault());
            classId++;

            var newClass = new Classes
            {
                ClassId = classId,
                CatalogId = catID,
                ProfessorId = instructor,
                Semester = season,
                StartTime = start,
                EndTime = end,
                Location = location
            };

            db.Classes.Add(newClass);
            db.SaveChanges();
            return Json(new { success = true });
        }


        /*******End code to modify********/

    }
}