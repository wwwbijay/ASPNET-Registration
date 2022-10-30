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

        public async Task<PartialViewResult> CheckPaymentStatus(string transaction_id)
        {

            //API request MyPay
            var API_URL = _mySettings.BASE_URL + "/api/use-mypay-payments-status";
            var API_KEY = _mySettings.API_KEY;

            var testBody2 = JsonContent.Create(new
            {
                MerchantTransactionId = transaction_id,
            });

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("API_KEY", API_KEY);

            using var httpResponseMessage = await client.PostAsync(API_URL, testBody2);

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseObj = JsonConvert.DeserializeObject<PaymentStatusResponse>(responseString);
                ViewBag.Message = responseObj.Remarks;
                return PartialView("~/Views/Account/_CheckPaymentStatus.cshtml");
            }
            else
            {
                ViewBag.Message = "Sorry. Couldnot get data from server...";
                return PartialView("~/Views/Account/_CheckPaymentStatus.cshtml");
            }

            
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("WpCookieMP");
            return RedirectToAction("Index");
        }

    }
}
