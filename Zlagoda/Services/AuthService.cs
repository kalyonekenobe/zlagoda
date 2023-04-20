using Zlagoda.Business.Interfaces;
using Zlagoda.Models;

namespace Zlagoda.Services
{
    public class AuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly PasswordService _passwordService;

		public AuthService(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor) 
        {
            JwtTokenService.HttpContextObject = httpContextAccessor;
            _employeeRepository = employeeRepository;
            _passwordService = new PasswordService();
        }

        public async Task<string> SignIn(string employeeId, string password)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            password = _passwordService.Encrypt(password);
            if (employee is null || !_passwordService.ComparePasswords(password, employee.empl_password))
            {
                throw new Exception("Wrong employee id or password!");
            }
            var token = JwtTokenService.GenerateRefreshToken(new User(employee));
            return token;
        }
    }
}
