
namespace LMS
{
    public class Utils
    {
        public static double GradeToScore(string grade)
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
    }
}
