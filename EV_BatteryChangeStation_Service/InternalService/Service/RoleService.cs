using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
        }
        // Tạo vai trò mới
        public async Task<IServiceResult> CreateRoleAsync(CreateRoleDTO createRole)
        {
            try
            {
                if (createRole == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var role = createRole.toModel();
                await _unitOfWork.RoleRepository.CreateAsync(role);

                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = Const.SUCCESS_CREATE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        // Xoá vai trò
        public async Task<IServiceResult> DeleteRoleAsync(Guid encodedId)
        {
            try
            {
                if (encodedId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(encodedId);
                if (role == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                await _unitOfWork.RoleRepository.RemoveAsync(role);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = Const.SUCCESS_DELETE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        // Lấy tất cả vai trò
        public async Task<IServiceResult> GetAllRolesAsync()
        {
            try
            {
                var role = await _unitOfWork.RoleRepository.GetAllRoleAsync();
                if (role == null || !role.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var result = role.Select(r => new ViewRoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Status = r.Status,
                    CreateDate = r.CreateDate,
                    UpdateDate = r.UpdateDate
                }).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = Const.SUCCESS_CREATE_MSG,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        ////lấy tất cả vai trò với mã id được mã hóa
        //public async Task<IServiceResult> GetAllRoleByIdDecodeAsync()
        //{
        //    try
        //    {
        //        var roles = await _unitOfWork.RoleRepository.GetAllRoleAsync();
        //        if (roles == null || !roles.Any())
        //        {
        //            return new ServiceResult
        //            {
        //                Status = Const.WARNING_NO_DATA_CODE,
        //                Message = Const.WARNING_NO_DATA_MSG,
        //            };
        //        }

        //        var result = roles.Select(r => new ViewRoleDto
        //        {
        //            RoleId = _hashids.Encode(r.RoleId), 
        //            RoleName = r.RoleName,
        //            Status = r.Status,
        //            CreateDate = r.CreateDate,
        //            UpdateDate = r.UpdateDate
        //        }).ToList();

        //        return new ServiceResult
        //        {
        //            Status = Const.SUCCESS_READ_CODE,
        //            Message = Const.SUCCESS_READ_MSG,
        //            Data = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult
        //        {
        //            Status = Const.ERROR_EXCEPTION,
        //            Message = ex.Message
        //        };
        //    }
        //}
        //lấy vai trò dựa vào tên
        public async Task<IServiceResult> GetRoleByNameAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var role = await _unitOfWork.RoleRepository.GetRoleByName(roleName);
                if (role == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = role.MaptoViewRoleDto()
                };
            }
            catch(Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        //cập nhật vai trò
        public async Task<IServiceResult> UpdateRoleAsync(UpdateRoleDTO updateRole)
        {
            try
            {
                if (updateRole == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(updateRole.RoleId);
                if (role == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }

                role.ToUpdateRoleFromDTO(updateRole);
                await _unitOfWork.RoleRepository.UpdateAsync(role);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_UPDATE_CODE,
                    Message = Const.SUCCESS_UPDATE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        // Xoá mềm vai trò
        public async Task<IServiceResult> SoftDeleteAsync(Guid encodedId)
        {
            try
            {
                if (encodedId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(encodedId);
                if (role == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                role.Status = false;
                await _unitOfWork.RoleRepository.UpdateAsync(role);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_UPDATE_CODE,
                    Message = Const.SUCCESS_UPDATE_MSG,
                    Data = role
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
    }
}
