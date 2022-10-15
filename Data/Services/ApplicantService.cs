using EventRegistration.Data.Base;
using EventRegistration.Models;

namespace EventRegistration.Data.Services
{
    public class ApplicantService: EntityBaseRepository<Applicant>, IApplicantService
    {
        public ApplicantService(AppDbContext context) : base(context) { }
    }
}
