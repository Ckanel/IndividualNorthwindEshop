using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommonData.Models;
using CommonData.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace IndividualNorthwindEshop.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Manager")]
    public class ManagerDashboardModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<ManagerDashboardModel> _logger;
        private readonly MasterContext _context;

        public ManagerDashboardModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            ILogger<ManagerDashboardModel> logger,
            MasterContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _emailStore = GetEmailStore();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The password must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Role is required")]
            public string Role { get; set; }

            [Required(ErrorMessage = "First Name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Address is required")]
            public string Address { get; set; }

            [Required(ErrorMessage = "City is required")]
            public string City { get; set; }

            public string Region { get; set; }

            [Required(ErrorMessage = "Postal Code is required")]
            public string PostalCode { get; set; }

            [Required(ErrorMessage = "Country is required")]
            public string Country { get; set; }

            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid Phone Number")]
            public string Phone { get; set; }
        }

     

        public async Task<IActionResult> OnPostAsync()
        {
            Debug.WriteLine("OnPostAsync called.");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state is invalid.");
                _logger.LogWarning("Form validation failed for user creation attempt with email {Email}.", Input?.Email);
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    if (existingUser.EmployeeId.HasValue)
                    {
                        _logger.LogWarning("User with email {Email} already has an associated EmployeeId.", Input.Email);
                        Debug.WriteLine($"User with email {Input.Email} already has an associated EmployeeId.");
                        ModelState.AddModelError(string.Empty, "An employee is already registered with this email.");
                        return Page();
                    }
                }

                var tempEmployee = new Employee
                {
                    FirstName = "-", // Just a placeholder to satisfy NOT NULL constraints if 'FirstName' column is VARCHAR(1)
                    LastName = "-",
                    Address = "-",
                    City = "-",
                    Region = "",
                    PostalCode = "",
                    Country = "",
                    HomePhone = ""
                };

                _context.Employees.Add(tempEmployee);
                await _context.SaveChangesAsync();
                Debug.WriteLine($"Temporary employee created with Id: {tempEmployee.EmployeeId}");

                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.EmployeeId = tempEmployee.EmployeeId;
                user.CustomerId = null; // Ensure that CustomerId remains null

                Debug.WriteLine($"Attempting to create a new user with email {Input.Email} and temporary EmployeeId {user.EmployeeId}");

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password for {Email}.", Input.Email);
                    Debug.WriteLine($"User created a new account with password for {Input.Email}");

                    await _userManager.AddToRoleAsync(user, Input.Role);
                    _logger.LogInformation("Assigned role {Role} to user {Email}.", Input.Role, Input.Email);
                    Debug.WriteLine($"Assigned role {Input.Role} to user {Input.Email}.");

                    if (Input.Role == "Manager" || Input.Role == "Employee")
                    {
                        tempEmployee.FirstName = Input.FirstName;
                        tempEmployee.LastName = Input.LastName;
                        tempEmployee.Address = Input.Address;
                        tempEmployee.City = Input.City;
                        tempEmployee.Region = Input.Region;
                        tempEmployee.PostalCode = Input.PostalCode;
                        tempEmployee.Country = Input.Country;
                        tempEmployee.HomePhone = Input.Phone;

                        _context.Employees.Update(tempEmployee);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Updated employee record for {FirstName} {LastName}.", Input.FirstName, Input.LastName);
                        Debug.WriteLine($"Updated employee record for {Input.FirstName} {Input.LastName}");
                    }

                    await transaction.CommitAsync();
                    return RedirectToPage("/Index");
                }

                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                foreach (var error in errorMessages)
                {
                    _logger.LogWarning("Error occurred while creating user {Email}: {ErrorDescription}", Input.Email, error);
                    Debug.WriteLine($"Error occurred while creating user {Input.Email}: {error}");
                    ModelState.AddModelError(string.Empty, error);
                }

                await transaction.RollbackAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user and employee. Rolling back transaction.");
                Debug.WriteLine($"An error occurred: {ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message);
                await transaction.RollbackAsync();
            }

            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create an instance of {UserType}.", nameof(User));
                Debug.WriteLine($"Failed to create an instance of {nameof(User)}: {ex.Message}");
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively override the register page in /Areas/Identity/Pages/Account/Register.cshtml", ex);
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }

    }
}