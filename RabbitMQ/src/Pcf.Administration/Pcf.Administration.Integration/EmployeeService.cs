using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.Integration
{
    public class EmployeeService(IRepository<Employee> employeeRepository) : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository = employeeRepository;

        public async Task AppliedPromocodesAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee is null)
                return;

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);
        }
    }
}
