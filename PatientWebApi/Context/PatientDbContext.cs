using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientWebApi.Models;

namespace PatientWebApi.Context
{
    public class PatientDbContext : IdentityDbContext
    {
        public PatientDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
    }
}
