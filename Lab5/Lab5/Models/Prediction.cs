using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    public class Prediction
    {
        public enum Question
        {
            Earth, Computer
        }
        
        public int PredictionId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Display(Name = "Question")]
        public Question question { get; set; }
    }
}
