using System.ComponentModel.DataAnnotations;

namespace ComplaintProj.Models
{
    public class ComplaintModel
    {
        public int Id { get; set; }


        // Patient Information
        [Required(ErrorMessage = "Patient name is required")]
        [Display(Name = "Patient Name")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Job is required")]
        public string? Job { get; set; }

        [Required(ErrorMessage = "Nationality is required")]
        public string? Nationality { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }


        // Complaint Details
        [Required(ErrorMessage = "Please select complaint type")]
        [Display(Name = "Complaint Type")]
        public string? ComplaintType { get; set; } // Medical or Non-Medical

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Complaint Category")]
        public string? ComplaintCategory { get; set; }

        [Required(ErrorMessage = "Please select complaint location")]
        [Display(Name = "Complaint Location")]
        public string? ComplaintLocation { get; set; }

        [Required(ErrorMessage = "Please provide a summary")]
        [StringLength(1000)]
        [Display(Name = "Complaint Summary")]
        public string? ComplaintSummary { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delet

        public string Status { get; set; } = "New";

        public string? AttachmentPath { get; set; }

        public string? AssignedStaffId { get; set; }

        public string? StaffReply { get; set; }

        public string? IsSatisfied { get; set; }

    }
}


