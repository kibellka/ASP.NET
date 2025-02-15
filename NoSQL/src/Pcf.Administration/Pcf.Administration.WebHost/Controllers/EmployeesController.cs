using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost.Models;

namespace Pcf.Administration.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository = employeeRepository;
        private readonly IRepository<Role> _roleRepository = roleRepository;

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IReadOnlyCollection<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по id
        /// </summary>
        /// <param name="id">Id сотрудника, например <example>6769d295ba2b0e10036a1b6a</example></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var role = await _roleRepository.GetByIdAsync(employee.RoleId);

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Role = new RoleItemResponse()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                },
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Обновить количество выданных промокодов
        /// </summary>
        /// <param name="id">Id сотрудника, например <example>6769d295ba2b0e10036a1b6a</example></param>
        /// <returns></returns>
        [HttpPost("{id}/appliedPromocodes")]

        public async Task<IActionResult> UpdateAppliedPromocodesAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);

            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteEmployeeAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            await _employeeRepository.DeleteAsync(employee);

            return NoContent();
        }
    }
}