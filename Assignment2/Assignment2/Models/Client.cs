using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class Client
    {
        public int Id { get; set; }

        
        [Required]
        [DisplayName("LastName")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("FirstName")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public string FullName { get { return (LastName + ", " + FirstName); } }

        public ICollection<Subscription> Subscriptions { get; set; }
        
    }
}
