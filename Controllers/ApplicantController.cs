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

namespace EventRegistration.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public ApplicantController(AppDbContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
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
                var ApiURL = "https://testapi.mypay.com.np/api/use-mypay-payments";
                var API_KEY = "ddOInxZbDO3OkH3fS9n7IIxywU94U1vPU25GeihGNaJN/gT6sBw4zXcknEFVRi0R";

                 var testBody2 = JsonContent.Create(new
                 {
                     Amount = checkout.Price,
                     OrderId = Convert.ToString(checkout.OrderId),
                     UserName = "testmerchant",
                     Password = "Tmerchant@123",
                     MerchantId = "MER29403933"
                 });

                var client = _clientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("API_KEY", API_KEY);

                using var httpResponseMessage = await client.PostAsync(ApiURL, testBody2);

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
                else { }
                
                return RedirectToAction(nameof(Index));
            }
            return View(checkout);

        }

        // GET: Applicants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            return View(applicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Phone,Email,Address,City,Province,PerformanceType,ParticipantName,Gender,Age,GroupName,NoOfMembers,AgeGroupRange,GroupType,Details")] Applicant applicant)
        {
            if (id != applicant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicantExists(applicant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicant);
        }

        // GET: Applicants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        }

        // POST: Applicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicants.Any(e => e.Id == id);
        }

        
    }
}
