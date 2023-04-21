using Zlagoda.Business.Interfaces;
using Zlagoda.Models;

namespace Zlagoda.Services
{
    public class AuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly PasswordService _passwordService;

		public AuthService(IEmployeeRepository employeeRepository) 
        {
            _employeeRepository = employeeRepository;
            _passwordService = new PasswordService();
        }

        public async Task<string> SignIn(string phone, string password)
        {
            var employee = await _employeeRepository.GetEmployeeByPhoneAsync(phone);
            password = _passwordService.Encrypt(password);
            if (employee is null || employee.empl_password is null || !_passwordService.ComparePasswords(password, employee.empl_password))
            {
                throw new Exception("Wrong employee phone or password!");
            }
            var user = new User(employee);
            var token = JwtTokenService.GenerateRefreshToken(user);
            return token;
        }
    }
}
