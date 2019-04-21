using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var result =
                from e in db.Enrollment
                join c in db.Classes on e.ClassId equals c.ClassId
                join crs in db.Courses on c.CatalogId equals crs.CatalogId
                join d in db.Departments on crs.DeptId equals d.DeptId
                where e.StudentId == uid
                select new
                {
                    subject = d.Abbrev,
                    number = crs.Number,
                    name = crs.Name,
                    season = c.Semester,
                    year = c.Year,
                    grade = e.Grade
                };
            return Json(result.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var result = from a in db.Assignments
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join e in db.Enrollment on c.ClassId equals e.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season 
                      && c.Year == year && e.StudentId == uid
                select new
                {
                    aname = a.Name,
                    cname = ac.Name,
                    due = a.DueDate,
                    score = db.Submissions.OrderByDescending(x => x.TimeSubmitted).FirstOrDefault(s => s.AsgId == a.AsgId && s.StudentId == uid).Score
                };
            return Json(result.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            bool madeIt;
            var assId = (from a in db.Assignments
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season 
                      && c.Year == year && ac.Name == category && a.Name == asgname
                select a.AsgId).FirstOrDefault();

            if (assId == 0)
            {
                madeIt = false;
            }
            else
            {
                //Remove html tags and add html line break
                contents = Regex.Replace(contents, @"<[^>]*>", String.Empty);
                contents = contents.Replace("\n", "<br/>");
                if (db.Submissions.Any(sb => sb.StudentId == uid && sb.AsgId == assId))
                {
                    Submissions current = db.Submissions.FirstOrDefault(sb => sb.StudentId == uid && sb.AsgId == assId);
                    current.Contents = contents;
                    current.TimeSubmitted = DateTime.Now;
                    db.Entry(current).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Submissions newSub = new Submissions
                    {
                        AsgId = assId,
                        StudentId = uid,
                        Contents = contents,
                        Score = 0,
                        TimeSubmitted = DateTime.Now
                    };
                    db.Submissions.Add(newSub);
                    db.SaveChanges();
                }

                madeIt = true;
            }

            return Json(new { success = madeIt });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            bool isNew = false;
            var result =
                from e in db.Enrollment
                join c in db.Classes on e.ClassId equals c.ClassId
                join crs in db.Courses on c.CatalogId equals crs.CatalogId
                join d in db.Departments on crs.DeptId equals d.DeptId
                where e.StudentId == uid && d.Abbrev == subject && Convert.ToInt16(crs.Number) == num
                      && c.Semester == season && c.Year == year
                select e;

            if (!result.Any())
            {
                int classId =
                    (from c in db.Classes
                        join crs in db.Courses on c.CatalogId equals crs.CatalogId
                        join d in db.Departments on crs.DeptId equals d.DeptId
                        where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num
                                                  && c.Semester == season && c.Year == year
                        select c.ClassId).FirstOrDefault();
                Enrollment enroll = new Enrollment
                {
                    ClassId = classId,
                    StudentId = uid
                };
                db.Enrollment.Add(enroll);
                db.SaveChanges();

                isNew = true;
            }
            return Json(new { success = isNew });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var score = new
            {
                gpa = 0.0
            };

            var grades = from e in db.Enrollment
                where e.StudentId == uid && !string.IsNullOrEmpty(e.Grade)
                select e.Grade;
            if (grades.Any())
            {
                List<double> scores = new List<double>();
                foreach (var grade in grades)
                {
                    scores.Add(Utils.GradeToGPA(grade));
                }

                double GPA = scores.Average();

                score = new
                {
                    gpa = GPA
                };
            }
            
            return Json(score);
        }

        /*******End code to modify********/

    }
}