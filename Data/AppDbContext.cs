using EventRegistration.Models;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Data
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }
        public DbSet<Applicant> Applicants { get; set; }
    }
}
