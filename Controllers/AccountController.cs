using EventRegistration.Data;
using EventRegistration.Data.Services;
using EventRegistration.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventRegistration.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly MyPaySettings _mySettings;

        private readonly IApplicantService _applicants;
        public AccountController(AppDbContext context, IApplicantService applicant_service, IHttpClientFactory clientFactory, IOptions<MyPaySettings> settings)
        {
            _context = context;
            _applicants = applicant_service;
            _clientFactory = clientFactory;
            _mySettings = settings.Value;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicantLists = await _applicants.GetAllAsync();
            return View(applicantLists);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([Bind("UserName, Password")] Credential credential)
        {
            if (!ModelState.IsValid)
            {
                return View(credential);
            }
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                //Creating the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Administrator"),
                    new Claim(ClaimTypes.Email, "admin@mypay.com.np"),
                };
                var identity = new ClaimsIdentity(claims, "WpCookieMP");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("WpCookieMP", claimsPrincipal);
                return RedirectToAction("Index");
            }

            return View(credential);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("WpCookieMP");
            return RedirectToAction("Index");
        }

    }
}
