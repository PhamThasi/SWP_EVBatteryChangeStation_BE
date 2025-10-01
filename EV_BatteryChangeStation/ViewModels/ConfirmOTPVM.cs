using System.ComponentModel.DataAnnotations;

namespace EV_BatteryChangeStation.ViewModels
{
    public class ConfirmOTPVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required.")]
        public string OTP { get; set; }
    }
}
