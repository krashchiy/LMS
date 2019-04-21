using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {

            var result =
                from u in db.Users
                join s in db.Students on u.UserId equals s.StudentId
                join e in db.Enrollment on s.StudentId equals e.StudentId
                join c in db.Classes on e.ClassId equals c.ClassId
                join crs in db.Courses on c.CatalogId equals crs.CatalogId
                where num == Convert.ToInt16(crs.Number) && season == c.Semester && year == c.Year
                select new
                {
                    fname = u.FirstName,
                    lname = u.LastName,
                    uid = u.UserId,
                    dob = u.Dob,
                    grade = e.Grade
                };

            return Json(result.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var result =
                from d in db.Departments
                join crs in db.Courses on d.DeptId equals crs.DeptId
                join c in db.Classes on crs.CatalogId equals c.CatalogId
                join a in db.AssignmentCategories on c.ClassId equals a.ClassId
                join asg in db.Assignments on a.AsgCatId equals asg.AsgCatId
                where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num && c.Semester == season && c.Year == year && a.Name == category
                select new
                {
                    aname = asg.Name,
                    cname = a.Name,
                    due = asg.DueDate,
                    submissions = db.Submissions.Count(sb => sb.AsgId == asg.AsgId)
                };

            return Json(result.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {

            var result =
                from d in db.Departments
                join crs in db.Courses on d.DeptId equals crs.DeptId
                join c in db.Classes on crs.CatalogId equals c.CatalogId
                join a in db.AssignmentCategories on c.ClassId equals a.ClassId
                where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num && c.Semester == season && c.Year == year
                select new
                {
                    name = a.Name,
                    weight = a.GradeWeight
                };

            return Json(result.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category,
            int catweight)
        {
            bool isNew = false;
            var result = from ac in db.AssignmentCategories
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season && c.Year == year && ac.Name == category
                select ac;

            if (!result.Any())
            {
                int classId =
                    (from c in db.Classes
                        join crs in db.Courses on c.CatalogId equals crs.CatalogId
                        join d in db.Departments on crs.DeptId equals d.DeptId
                        where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num && c.Semester == season && c.Year == year
                        select c.ClassId).FirstOrDefault();
                var asgCat = new AssignmentCategories
                {
                    ClassId = classId,
                    GradeWeight = Convert.ToByte(catweight),
                    Name = category
                };

                db.AssignmentCategories.Add(asgCat);
                db.SaveChanges();
                isNew = true;
            }
        
            return Json(new { success = isNew });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            bool madeIt = false;
            var assCatId = (from ac in db.AssignmentCategories
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season 
                      && c.Year == year && ac.Name == category
                select ac.AsgCatId).FirstOrDefault();

            
            if (assCatId > 0 && !db.Assignments.Any(a => a.AsgCatId == assCatId && a.Name == asgname))
            {
                var assignment = new Assignments
                {
                    AsgCatId = assCatId,
                    Contents = asgcontents,
                    DueDate = asgdue,
                    MaxPoints = asgpoints,
                    Name = asgname
                };

                db.Assignments.Add(assignment);
                db.SaveChanges();

                int classId =
                    (from c in db.Classes
                        join crs in db.Courses on c.CatalogId equals crs.CatalogId
                        join d in db.Departments on crs.DeptId equals d.DeptId
                        where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num && c.Semester == season && c.Year == year
                        select c.ClassId).FirstOrDefault();

                //Get all students in this class and update their grades accordingly
                if (db.Enrollment.Any(e => e.ClassId == classId))
                {
                    List<string> studIds = db.Enrollment.Where(e => e.ClassId == classId).Select(e => e.StudentId).ToList();
                    foreach (var uid in studIds)
                    {
                        UpdateStudentGrade(uid, classId);
                    }
                }
                madeIt = true;
            }

            return Json(new { success = madeIt });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var result = from sm in db.Submissions
                join a in db.Assignments on sm.AsgId equals a.AsgId
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                join u in db.Users on sm.StudentId equals u.UserId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season
                      && c.Year == year && ac.Name == category && a.Name == asgname
                select new
                {
                    fname = u.FirstName,
                    lname = u.LastName,
                    uid = sm.StudentId,
                    time = sm.TimeSubmitted,
                    score = sm.Score
                };
            return Json(result.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            bool foundIt = false;
            var submission = from sm in db.Submissions
                join a in db.Assignments on sm.AsgId equals a.AsgId
                join ac in db.AssignmentCategories on a.AsgCatId equals ac.AsgCatId
                join c in db.Classes on ac.ClassId equals c.ClassId
                join cr in db.Courses on c.CatalogId equals cr.CatalogId
                join d in db.Departments on cr.DeptId equals d.DeptId
                where d.Abbrev == subject && Convert.ToInt16(cr.Number) == num && c.Semester == season
                      && c.Year == year && ac.Name == category && a.Name == asgname && sm.StudentId == uid
                select sm;
            if (submission.Any())
            {
                //Grade the latest submission
                var latestSub = submission.OrderByDescending(sb => sb.TimeSubmitted).FirstOrDefault();
                latestSub.Score = score;
                db.Entry(latestSub).State = EntityState.Modified;
                db.SaveChanges();

                int classId =
                    (from c in db.Classes
                        join crs in db.Courses on c.CatalogId equals crs.CatalogId
                        join d in db.Departments on crs.DeptId equals d.DeptId
                        where d.Abbrev == subject && Convert.ToInt16(crs.Number) == num && c.Semester == season && c.Year == year
                        select c.ClassId).FirstOrDefault();

                //Update student's grade for the class
                UpdateStudentGrade(uid, classId);

                foundIt = true;
            }
            return Json(new { success = foundIt });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var result =
                from c in db.Classes
                join crs in db.Courses on c.CatalogId equals crs.CatalogId
                join d in db.Departments on crs.DeptId equals d.DeptId
                join p in db.Professors on c.ProfessorId equals p.ProfessorId
                where c.ProfessorId == uid
                select new
                {
                    subject = d.Abbrev,
                    number = crs.Number,
                    name = crs.Name,
                    season = c.Semester,
                    year = c.Year
                };

            return Json(result.ToArray());
        }

        private void UpdateStudentGrade(string uid, int classId)
        {
            //Get only latest submissions for each assignment id for a student
                var subs = from sm in db.Submissions
                    where sm.StudentId == uid
                    group sm by sm.AsgId
                    into g
                    select new
                    {
                        subId = (from tb in g select tb.SubmissionId).Max()
                    };
                var subIds = subs.Select(s => s.subId).ToList();

                var studentSubs = from sm in db.Submissions
                    join a in db.Assignments on sm.AsgId equals a.AsgId
                    where subIds.Contains(sm.SubmissionId)
                    group new {a, sm} by a.AsgCatId
                    into g
                    join ac in db.AssignmentCategories on g.Key equals ac.AsgCatId
                    select new
                    {
                        weight = ac.GradeWeight,
                        earnedTotal = (from tb in g select tb.sm.Score ?? 0).Sum(),
                        maxTotal = (from tb in g select tb.a.MaxPoints).Sum()
                    };

                double totalScore = 0;
                double weightsTotal = studentSubs.Select(x => Convert.ToDouble(x.weight)).Sum();
                foreach (var catSub in studentSubs)
                {
                    double catGrade = Math.Round(Convert.ToDouble(catSub.earnedTotal / catSub.maxTotal), 2);
                    double catScore = Convert.ToDouble(catGrade * catSub.weight);
                    totalScore += catScore;
                }

                double scaleFactor = Math.Round(100 / weightsTotal, 2);
                totalScore = Math.Round(totalScore * scaleFactor, 2);
                if (totalScore > 100)
                {
                    totalScore = 100;
                }

                string grade = Utils.ScoreToGrade(totalScore);
                
                db.Enrollment.FirstOrDefault(e => e.StudentId == uid && e.ClassId == classId).Grade=grade;
                db.SaveChanges();
        }

        /*******End code to modify********/

    }
}