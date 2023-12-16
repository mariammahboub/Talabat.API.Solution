using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Employee_Specs;

namespace Talabat.API.Controllers
{
    public class EmployeesController : ApiBaseController
    {
        #region Ctor for Create Obj from generic Class 
        private readonly IGenericRepository<Employee> _employeeRepository;

        public EmployeesController(IGenericRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        #endregion

        #region End Points

        #region Get All Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetProducts()
        {
            var spec = new EmployeeWithDepartmentSpecifications();
            var Employees = await _employeeRepository.GetAllWithSpecAsync(spec);

            return Ok(Employees);
        }
        #endregion

        #region Get Employee
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var spec = new EmployeeWithDepartmentSpecifications(id);
            var Employee = await _employeeRepository.GetWithSpecAsync(spec);
            if (Employee == null)
                return NotFound(new ApiResponse(404));
            return Ok(Employee);
        }

        #endregion

        #endregion
    }
}
