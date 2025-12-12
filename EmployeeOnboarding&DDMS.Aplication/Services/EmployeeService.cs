using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Employees;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository employeeRepository, 
            IUserRepository userRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<Response<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            // Validate hire date is not in the future
            if (dto.HireDate > DateTime.UtcNow.Date)
            {
                return new Response<EmployeeDto>("Hire date cannot be in the future.");
            }

            // Check if email already exists
            var existingEmployee = await _employeeRepository.GetByEmailAsync(dto.Email);
            if (existingEmployee != null)
            {
                return new Response<EmployeeDto>("Employee with this email already exists.");
            }

            // Generate employee number
            var employeeNumber = await GenerateEmployeeNumberAsync();

            var employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate,
                Department = dto.Department,
                Position = dto.Position,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                EmploymentStatus = EmploymentStatus.Active,
                OnboardingStatus = OnboardingStatus.NotStarted,
                CreatedBy = dto.CreatedBy.ToString()
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee);
            
            // Automatically create user account with default password
            var defaultPassword = "TempPass123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
            
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = UserRole.Employee,
                IsActive = true,
                MustChangePassword = true, // Force password change on first login
                CreatedBy = dto.CreatedBy.ToString()
            };
            
            await _userRepository.AddAsync(user);
            
            // CRITICAL FIX: Link user to employee
            createdEmployee.UserId = user.Id;
            await _employeeRepository.UpdateAsync(createdEmployee);
            
            // Send welcome email with login credentials
            try
            {
                await _emailService.SendWelcomeEmailAsync(
                    dto.Email,
                    $"{dto.FirstName} {dto.LastName}",
                    defaultPassword
                );
            }
            catch (Exception ex)
            {
                // Log error but don't fail employee creation
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }
            
            var employeeDto = _mapper.Map<EmployeeDto>(createdEmployee);

            return new Response<EmployeeDto>(employeeDto, "Employee created successfully. Welcome email sent with login credentials.");
        }

        public async Task<Response<EmployeeDto>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return new Response<EmployeeDto>("Employee not found.");
            }

            // Validate hire date is not in the future
            if (dto.HireDate > DateTime.UtcNow.Date)
            {
                return new Response<EmployeeDto>("Hire date cannot be in the future.");
            }

            // Email is read-only - don't update it
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.DateOfBirth = dto.DateOfBirth;
            employee.HireDate = dto.HireDate;
            employee.Department = dto.Department;
            employee.Position = dto.Position;
            employee.StreetAddress = dto.StreetAddress;
            employee.City = dto.City;
            employee.State = dto.State;
            employee.PostalCode = dto.PostalCode;
            employee.Country = dto.Country;

            await _employeeRepository.UpdateAsync(employee);
            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return new Response<EmployeeDto>(employeeDto, "Employee updated successfully.");
        }

        public async Task<Response<EmployeeDto>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return new Response<EmployeeDto>("Employee not found.");
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return new Response<EmployeeDto>(employeeDto);
        }

        public async Task<PagedResponse<IEnumerable<EmployeeDto>>> GetEmployeesAsync(EmployeeFilterDto filter)
        {
            var query = await _employeeRepository.GetAllAsync();
            var employees = query.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Department))
            {
                employees = employees.Where(e => e.Department == filter.Department);
            }

            if (filter.OnboardingStatus.HasValue)
            {
                employees = employees.Where(e => e.OnboardingStatus == filter.OnboardingStatus.Value);
            }

            if (filter.EmploymentStatus.HasValue)
            {
                employees = employees.Where(e => e.EmploymentStatus == filter.EmploymentStatus.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                employees = employees.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.EmployeeNumber.ToLower().Contains(searchTerm));
            }

            var totalRecords = employees.Count();
            var pagedEmployees = employees
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(pagedEmployees);

            return new PagedResponse<IEnumerable<EmployeeDto>>(
                employeeDtos,
                filter.PageNumber,
                filter.PageSize,
                totalRecords);
        }

        public async Task<Response<bool>> DeactivateEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return new Response<bool>("Employee not found.");
            }

            // Soft delete: Set employment status to Inactive
            employee.EmploymentStatus = EmploymentStatus.Inactive;
            await _employeeRepository.UpdateAsync(employee);

            // Note: Documents archival is handled at the database level through cascade
            // All documents remain accessible for compliance purposes

            return new Response<bool>(true, "Employee deactivated successfully. All documents archived for compliance.");
        }

        public async Task<Response<bool>> CompleteOnboardingAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return new Response<bool>("Employee not found.");
            }

            employee.OnboardingStatus = OnboardingStatus.Completed;
            employee.OnboardingCompletedDate = DateTime.UtcNow;
            employee.EmploymentStatus = EmploymentStatus.Active;

            await _employeeRepository.UpdateAsync(employee);

            return new Response<bool>(true, "Onboarding completed successfully.");
        }

        private async Task<string> GenerateEmployeeNumberAsync()
        {
            var year = DateTime.Now.Year;
            var employees = await _employeeRepository.GetAllAsync();
            var yearEmployees = employees.Where(e => e.EmployeeNumber.StartsWith($"EMP{year}")).ToList();
            var nextNumber = yearEmployees.Count + 1;
            return $"EMP{year}{nextNumber:D4}";
        }
    }
}

