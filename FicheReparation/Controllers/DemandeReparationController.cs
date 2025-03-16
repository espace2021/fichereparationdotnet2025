using FicheReparation.Entity;
using FicheReparation.Models;
using Microsoft.AspNetCore.Mvc;


namespace FicheReparation.Controllers
{
    public class DemandeReparationController : Controller
    {
        private readonly IDemandeReparationRepository _demandeReparationRepository;
        private readonly IClientRepository _clientRepository; // Ajout d'une référence à un repository pour les clients


        public DemandeReparationController(IDemandeReparationRepository demandeReparationRepository, IClientRepository clientRepository)
        {
            _demandeReparationRepository = demandeReparationRepository;
            _clientRepository = clientRepository;
        }

    
        // GET: DemandeReparation
        public async Task<IActionResult> Index()
        {
            var demandes = await _demandeReparationRepository.GetAllAsync();
            return View(demandes);
        }

        // GET: DemandeReparation/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var demande = await _demandeReparationRepository.GetByIdAsync(id);
            if (demande == null)
            {
                return NotFound();
            }
            return View(demande);
        }

        // GET: DemandeReparation/Create
        public async Task<IActionResult> Create()
        {
            // Récupérer la liste des clients via le repository
            var clients = await _clientRepository.GetAllClientsAsync(); //  méthode  dans le repository Client
            ViewData["Clients"] = clients;

            return View();
        }

        // POST: DemandeReparation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DateDepotAppareil,Appareil,Etat,SymptomesPanne,ClientId")] DemandeReparation demandeReparation)
        {
            if (ModelState.IsValid)
            {
                await _demandeReparationRepository.AddAsync(demandeReparation);
                return RedirectToAction("Index", "DemandeReparation");
            }
            return View(demandeReparation);
        }

        // GET: DemandeReparation/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var demande = await _demandeReparationRepository.GetByIdAsync(id);
            if (demande == null)
            {
                return NotFound();
            }
            return View(demande);
        }

        // POST: DemandeReparation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateDepotAppareil,Appareil,Etat,SymptomesPanne,ClientId")] DemandeReparation demandeReparation)
        {
            if (id != demandeReparation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _demandeReparationRepository.UpdateAsync(demandeReparation);
                return RedirectToAction(nameof(Index));
            }
            return View(demandeReparation);
        }

        // GET: DemandeReparation/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var demande = await _demandeReparationRepository.GetByIdAsync(id);
            if (demande == null)
            {
                return NotFound();
            }
            return View(demande);
        }

        // POST: DemandeReparation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _demandeReparationRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: DemandeReparation/ClientDemandes/5
        public async Task<IActionResult> ClientDemandes(int clientId)
        {
            var demandes = await _demandeReparationRepository.GetByClientIdAsync(clientId);
            return View(demandes);
        }
    }
}
