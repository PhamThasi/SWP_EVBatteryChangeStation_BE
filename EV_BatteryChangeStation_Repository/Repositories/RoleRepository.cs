using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository() {}

        public RoleRepository(EVBatterySwapContext context) => _context = context;

        public Task<List<Role>> GetAllRoleAsync()
        {
            return _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleByName(string name)
        {
            string normalizedRoleName = name.Trim().ToLower();
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == normalizedRoleName);
        }
    }
}
