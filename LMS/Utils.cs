
namespace LMS
{
    public class Utils
    {
        public static double GradeToGPA(string grade)
        {
            double score;
            switch (grade)
            {
                 case "A":
                     score = 4;
                     break;
                case "A-":
                    score = 3.7;
                    break;
                case "B+":
                    score = 3.3;
                    break;
                case "B":
                    score = 3;
                    break;
                case "B-":
                    score = 2.7;
                    break;
                case "C+":
                    score = 2.3;
                    break;
                case "C":
                    score = 2.0;
                    break;
                case "C-":
                    score = 1.7;
                    break;
                case "D+":
                    score = 1.3;
                    break;
                case "D":
                    score = 1;
                    break;
                case "D-":
                    score = 0.7;
                    break;
                default:
                    score = 0;
                    break;
            }

            return score;
        }

        public static string ScoreToGrade(double score)
        {
            string grade;
            if (93 <= score && score <= 100)
            {
                grade = "A";
            }
            else if (90 <= score && score < 93)
            {
                grade =  "A-";
            }
            else if (87 <= score && score < 90)
            {
                grade =  "B+";
            }
            else if (83 <= score && score < 87)
            {
                grade =  "B";
            }
            else if (80 <= score && score < 83)
            {
                grade =  "B-";
            }
            else if (77 <= score && score < 80)
            {
                grade =  "C+";
            }
            else if (73 <= score && score < 77)
            {
                grade =  "C";
            }
            else if (70 <= score && score < 73)
            {
                grade =  "C-";
            }
            else if (67 <= score && score < 70)
            {
                grade =  "D+";
            }
            else if (63 <= score && score < 67)
            {
                grade =  "D";
            }
            else if (60 <= score && score < 63)
            {
                grade =  "D-";
            }
            else
            {
                grade = "E";
            }

            return grade;
        }
    }
}
