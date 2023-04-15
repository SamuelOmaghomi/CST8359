using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class News
    {
        public int NewsId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Image")]
        public string Url { get; set; }

        public NewsBoard NewsBoard { get; set; }
    }
}
