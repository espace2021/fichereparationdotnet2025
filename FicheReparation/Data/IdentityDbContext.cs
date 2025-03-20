using Microsoft.EntityFrameworkCore;

namespace FicheReparation.Data
{
    public class IdentityDbContext<T>
    {
        private DbContextOptions<ApplicationDbContext> options;

        public IdentityDbContext(DbContextOptions<ApplicationDbContext> options)
        {
            this.options = options;
        }
    }
}