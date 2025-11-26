using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.AccountDto
{
    public class UpdateAccountDTO
    {
        public Guid AccountId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? StationId { get; set; }
        public string? AccountName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
