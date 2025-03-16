using FicheReparation.Models;
using Microsoft.EntityFrameworkCore;

namespace FicheReparation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Client> Clients { get; set; }
    }
}
