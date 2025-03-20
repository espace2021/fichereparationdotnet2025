using FicheReparation.Data;
using FicheReparation.Models;
using Microsoft.EntityFrameworkCore;

namespace FicheReparation.Entity
{
    public class DemandeReparationRepository : IDemandeReparationRepository
    {
        private readonly ApplicationDbContext _context;

        public DemandeReparationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DemandeReparation>> GetAllAsync()
        {
            return await _context.DemandeReparations.Include(d => d.Client).ToListAsync();
        }

        public async Task<DemandeReparation> GetByIdAsync(int id)
        {
            return await _context.DemandeReparations.Include(d => d.Client)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DemandeReparation>> GetByClientIdAsync(int clientId)
        {
            return await _context.DemandeReparations
                .Where(d => d.ClientId == clientId)
                .ToListAsync();
        }

        public async Task AddAsync(DemandeReparation demande)
        {
            await _context.DemandeReparations.AddAsync(demande);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DemandeReparation demande)
        {
            _context.DemandeReparations.Update(demande);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var demande = await _context.DemandeReparations.FindAsync(id);
            if (demande != null)
            {
                _context.DemandeReparations.Remove(demande);
                await _context.SaveChangesAsync();
            }
        }

      }
}
