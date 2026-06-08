using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComplaintProj.ViewModel
{
    public class ComplaintViewModel
    {
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
        public string? ComplaintType { get; set; }

        // here is the diff to can use checkbox
        [Required(ErrorMessage = "Please select at least one category")]
        [Display(Name = "Complaint Category")]
        public List<string> ComplaintCategory { get; set; } = new List<string>();

        [Required(ErrorMessage = "Please select complaint location")]
        [Display(Name = "Complaint Location")]
        public string? ComplaintLocation { get; set; }

        [Required(ErrorMessage = "Please provide a summary")]
        [StringLength(1000)]
        [Display(Name = "Complaint Summary")]
        public string? ComplaintSummary { get; set; }
    }
}
