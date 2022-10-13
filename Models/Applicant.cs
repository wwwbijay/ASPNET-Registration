using EventRegistration.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventRegistration.Models
{
    public class Applicant
    { 
        [Key]
        public int Id { get; set; }
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "The Full Name is required")]
        public string FullName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        [Display(Name = "Performance Type")]
        [Required(ErrorMessage = "Please select performance type")]
        public string PerformanceType { get; set; }
        [Display(Name = "Participant Name")]
        public string ParticipantName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        [Display(Name = "No Of Members")]
        public string NoOfMembers { get; set; }
        [Display(Name = "Age Group Range")]
        public string AgeGroupRange { get; set; }
        [Display(Name = "Group Type")]
        public string GroupType { get; set; }
        public string Details { get; set; }
        public long OrderId { get; set; }
        public string TransactionId { get; set; }
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }

    }
}
