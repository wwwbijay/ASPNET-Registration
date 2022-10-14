using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventRegistration.Data;
using EventRegistration.Models;
using System.Reflection.Metadata;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace EventRegistration.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly MyPaySettings _mySettings;

        public ApplicantController(
            AppDbContext context, 
            IHttpClientFactory clientFactory,
            IOptions<MyPaySettings> settings
            )
        {
            _context = context;
            _clientFactory = clientFactory;
            _mySettings = settings.Value;
        }

        // GET: Applicants
        public IActionResult Index()
        {
            
            var data = new string[7] {
                "Province 1", 
                "Madhesh Province", 
                "Bagmati Pradesh", 
                "Gandaki Province", 
                "Lumbini Province", 
                "Karnali Province", 
                "Sudurpashchim Province" 
            };

            ViewBag.Province = new List<string>(data);
            return View();
        }
        
        // POST: Applicants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("FullName, Phone, Email, Address, City, Province, PerformanceType, ParticipantName, Gender, Age, GroupName, NoOfMembers, AgeGroupRange, GroupType, Details, OrderId, TransactionId, PaymentStatus")] Applicant applicant)
        {
            if (ModelState.IsValid)
            {
                applicant.PaymentStatus = Data.Enums.PaymentStatus.Pending;
                _context.Add(applicant);
                await _context.SaveChangesAsync();
                var id = applicant.Id;
                applicant.OrderId = id + 1111111;
                _context.Update(applicant);
                await _context.SaveChangesAsync();

                TempData["ID"] = applicant.Id;
       
                return RedirectToAction(nameof(Checkout));
            }
            var data = new string[7] {
                "Province 1",
                "Madhesh Province",
                "Bagmati Pradesh",
                "Gandaki Province",
                "Lumbini Province",
                "Karnali Province",
                "Sudurpashchim Province"
            };

            ViewBag.Province = new List<string>(data);

            return View(applicant);
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var orderId = TempData["ID"];

            var applicant = await _context.Applicants.FindAsync(orderId);
            if (applicant == null)
            {
                return NotFound();
            }

            ViewBag.applicant_Id = applicant.Id;
            ViewBag.applicant_orderId = 1111111 + applicant.Id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout([Bind("ApplicantId, OrderId, pType, Price")] Checkout checkout )
        {
            if (ModelState.IsValid)
            {
                //API request MyPay
                var API_URL = _mySettings.BASE_URL + "/api/use-mypay-payments";
                var API_KEY = _mySettings.API_KEY;

                 var testBody2 = JsonContent.Create(new
                 {
                     Amount = checkout.Price,
                     OrderId = Convert.ToString(checkout.OrderId),
                     UserName = _mySettings.UserName,
                     Password = _mySettings.Password,
                     MerchantId = _mySettings.MerchantId
                 });

                var client = _clientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("API_KEY", API_KEY);

                using var httpResponseMessage = await client.PostAsync(API_URL, testBody2);

                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var applicant = await _context.Applicants.FindAsync(checkout.ApplicantId);

                    if (applicant == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var responseObj = JsonConvert.DeserializeObject<PaymentResponse>(responseString);
                        var redirectUrl = responseObj.RedirectURL;
                        applicant.TransactionId = responseObj.MerchantTransactionId;
                        _context.Update(applicant);
                        await _context.SaveChangesAsync();
                        return Redirect(redirectUrl);
                    }
                   
                }
                else {
                    return View(checkout);
                }
                
               
            }

            return View(checkout);

        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failed()
        {
            return View();
        }
     

        private bool ApplicantExists(int id)
        {
            return _context.Applicants.Any(e => e.Id == id);
        }

        
    }
}
