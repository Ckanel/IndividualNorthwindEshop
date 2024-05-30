// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CommonData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CommonData.Data;
using System.Security.Claims;
using Azure.Core;
using IndividualNorthwindEshop.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Policy;

namespace IndividualNorthwindEshop.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly MasterContext _context;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            MasterContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context= context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }
            [Display(Name = "Contact Name")]
            public string ContactName { get; set; }

            [Display(Name = "Contact Title")]
            public string ContactTitle { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Required]
            [Display(Name = "Country")]
            public string Country { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone")]
            public string Phone { get; set; }
        }





        //    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");
        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        //    if (ModelState.IsValid)
        //    {
        //        var user = CreateUser();

        //        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        //        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        //            var userType = Request.Form["userType"].ToString();

        //            // Generate a unique customer ID based on the user type
        //            string customerId = userType == "business"
        //            ? GenerateCustomerId(Input.CompanyName)
        //            : GenerateCustomerId("CUSTOMER");

        //            // Create a new customer record


        //            if (userType == "business")
        //            {
        //                // Store the company name in the business table
        //                var business = new Customer
        //                {
        //                    CustomerId = customerId,
        //                    CompanyName = Input.CompanyName,
        //                    ContactName = Input.ContactName,
        //                    ContactTitle = Input.ContactTitle,
        //                    Address = Input.Address,
        //                    City = Input.City,
        //                    PostalCode = Input.PostalCode,
        //                    Country = Input.Country,
        //                    Phone = Input.Phone
        //                };
        //                user.Customer = business;
        //            }
        //            else if (userType == "customer")
        //            {
        //                // Store the user type in the CompanyName column of the customer table
        //                var customer = new Customer
        //                {
        //                    CustomerId = customerId,
        //                    CompanyName = userType,
        //                    ContactName = Input.FirstName + " " + Input.LastName,
        //                    ContactTitle = null,
        //                    Address = Input.Address,
        //                    City = Input.City,
        //                    PostalCode = Input.PostalCode,
        //                    Country = Input.Country,
        //                    Phone = Input.Phone
        //                };

        //                user.Customer = customer;
        //            }



        //        var result = await _userManager.CreateAsync(user, Input.Password);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User created a new account with password.");
        //            await _userManager.AddToRoleAsync(user, "Customer");
        //            await _userManager.AddClaimAsync(user, new Claim("CustomerId", customerId));

        //            // Creates a new cart and associates it with the customer
        //            var cart = new Cart
        //            {
        //                CustomerId = customerId
        //            };
        //            _context.Carts.Add(cart);
        //            await _context.SaveChangesAsync();

        //            var userId = await _userManager.GetUserIdAsync(user);
        //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //            var callbackUrl = Url.Page(
        //            "/Account/ConfirmEmail",
        //            pageHandler: null,
        //                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
        //                protocol: Request.Scheme);

        //            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
        //                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //            if (_userManager.Options.SignIn.RequireConfirmedAccount)
        //            {
        //                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
        //            }
        //            else
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                TempData["SuccessMessage"] = "Registration successful. You are now logged in.";
        //                return LocalRedirect(returnUrl ?? Url.Content("~/"));
        //            }
        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Page();
        //}
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var userType = Request.Form["userType"].ToString();
                string customerId = userType == "business"
                                    ? GenerateCustomerId(Input.CompanyName)
                                    : GenerateCustomerId("CUSTOMER");

                if (userType == "business")
                {
                    var business = new Customer
                    {
                        CustomerId = customerId,
                        CompanyName = Input.CompanyName,
                        ContactName = Input.ContactName,
                        ContactTitle = Input.ContactTitle,
                        Address = Input.Address,
                        City = Input.City,
                        PostalCode = Input.PostalCode,
                        Country = Input.Country,
                        Phone = Input.Phone
                    };
                    user.Customer = business;
                }
                else if (userType == "customer")
                {
                    var customer = new Customer
                    {
                        CustomerId = customerId,
                        CompanyName = userType,
                        ContactName = Input.FirstName + " " + Input.LastName,
                        ContactTitle = null,
                        Address = Input.Address,
                        City = Input.City,
                        PostalCode = Input.PostalCode,
                        Country = Input.Country,
                        Phone = Input.Phone
                    };
                    user.Customer = customer;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _userManager.AddClaimAsync(user, new Claim("CustomerId", customerId));

                    var cart = new Cart
                    {
                        CustomerId = customerId
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code },
                        protocol: Request.Scheme);

                    _logger.LogInformation("Generated email confirmation URL: {Url}", callbackUrl);
                    var emailBody = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>";
                    _logger.LogInformation("Email body: {EmailBody}", emailBody);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
                    

                  



                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Registration successful. Please check your email to confirm your account.";
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }





        private string GenerateCustomerId(string input)
    {
        string customerId = string.Empty;

        if (!string.IsNullOrEmpty(input))
        {
            // Extracts the first five uppercase letters from the input
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    customerId += c;
                    if (customerId.Length == 5)
                        break;
                }
            }
        }

        // If the input is null, empty, or has less than five uppercase letters, generate a random customer ID
        if (customerId.Length < 5)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            customerId = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Check if the generated customer ID already exists in the database
        while (_context.Customers.Any(c => c.CustomerId == customerId))
        {
            // If the customer ID already exists, generate a new random customer ID
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            customerId = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        return customerId;
    }



    






    private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
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
