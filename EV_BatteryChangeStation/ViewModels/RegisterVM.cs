using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;

namespace EV_BatteryChangeStation.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Account ID")]
        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }

        [Display(Name = "Account Name")]
        [MaxLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
        public string AccountName { get; set; }

        [Display(Name = "Full Name")]
        [MaxLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Invalid VN phone number format.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOnly? DateOfBirth { get; set; }


        public string? OTP { get; set; }
    }
}
