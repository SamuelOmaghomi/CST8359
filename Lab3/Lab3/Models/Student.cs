using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
{
    public class Student
    {
        [Required]
        public String firstName
        {
            get; set;
        }
        [Required]
        public String lastName
        {
            get; set;
        }
        [Required]
        public int studentId
        {
            get; set;
        }
        [Required]
        public String emailAddress
        {
            get; set;
        }
        [Required]
        public String password
        {
            get; set;
        }
        [Required]
        public String studentDescription
        {
            get; set;
        }
    }
}
