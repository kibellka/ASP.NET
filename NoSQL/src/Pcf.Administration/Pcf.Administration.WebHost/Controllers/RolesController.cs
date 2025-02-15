using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.Administration.WebHost.Models;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController(IRepository<Role> roleRepository)
        : ControllerBase
    {
        private readonly IRepository<Role> _roleRepository = roleRepository;
        
        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IReadOnlyCollection<RoleItemResponse>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            var rolesModelList = roles.Select(x =>
                new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

            return rolesModelList;
        }
    }
}