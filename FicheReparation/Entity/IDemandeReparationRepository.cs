using FicheReparation.Models;
namespace FicheReparation.Entity
{
    public interface IDemandeReparationRepository
    {
        Task<IEnumerable<DemandeReparation>> GetAllAsync();
        Task<DemandeReparation> GetByIdAsync(int id);
        Task<IEnumerable<DemandeReparation>> GetByClientIdAsync(int clientId);
        Task AddAsync(DemandeReparation demande);
        Task UpdateAsync(DemandeReparation demande);
        Task DeleteAsync(int id);

    }
}
