using FicheReparation.Entity;
using FicheReparation.Helpers;
using FicheReparation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace FicheReparation.Controllers
{
    public class DemandeReparationController : Controller
    {
        private readonly IDemandeReparationRepository _demandeReparationRepository;
        private readonly IClientRepository _clientRepository; // Ajout d'une référence à un repository pour les clients
        private readonly PdfService _pdfService; //pdf
        private readonly IEmailService _emailService; //send email


        public ActionDescriptor ActionDescriptor { get; internal set; }

        public DemandeReparationController(IDemandeReparationRepository demandeReparationRepository, IClientRepository clientRepository, PdfService pdfService , IEmailService emailService)
        {
            _demandeReparationRepository = demandeReparationRepository;
            _clientRepository = clientRepository;
            _pdfService = pdfService;
            _emailService = emailService;
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

        public ControllerContext GetControllerContext()
        {
            return ControllerContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DateDepotAppareil,Appareil,Etat,SymptomesPanne,ClientId")] DemandeReparation demandeReparation)
        {
            if (ModelState.IsValid)
            {
                // Récupération des informations du client
                var client = await _clientRepository.GetClientByIdAsync(demandeReparation.ClientId);
                if (client == null)
                {
                    ModelState.AddModelError("ClientId", "Client non trouvé.");
                    ViewData["Clients"] = await _clientRepository.GetAllClientsAsync();
                    return View(demandeReparation);
                }

                // Associer le nom du client à la demande de réparation
                demandeReparation.Client = client;

                // Sauvegarde de la demande de réparation
                await _demandeReparationRepository.AddAsync(demandeReparation);

                // Génération du PDF avec la vue "IndexPdf" et le modèle contenant le client
                var pdfBytes = _pdfService.GeneratePdfFromView("IndexPdf", demandeReparation, ControllerContext);

                // Retour du fichier PDF avec le nom du client dans le nom du fichier
                string fileName = $"DemandeReparation_{client.Nom}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }

            // En cas d'erreur, recharger la liste des clients
            ViewData["Clients"] = await _clientRepository.GetAllClientsAsync();
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
                // Vérifier si l’état est "réparé"
                if (demandeReparation.Etat.ToLower() == "réparé")
                {
                    // Récupérer l'email du client (à adapter selon votre modèle)
                    var client = await _clientRepository.GetClientByIdAsync(demandeReparation.ClientId);
                    if (client != null && !string.IsNullOrEmpty(client.Email))
                    {
                        string subject = "Votre appareil est réparé !";
                        string body = $"<h1>Bonjour {client.Nom},</h1><p>Votre appareil <strong>{demandeReparation.Appareil}</strong> a été réparé avec succès. Vous pouvez venir le récupérer.</p>";

                        await _emailService.SendEmailAsync(client.Email, subject, body);
                    }
                }

                await _demandeReparationRepository.UpdateAsync(demandeReparation);
                return RedirectToAction(nameof(Index));
            }

            return View(demandeReparation);
        }



        // POST: DemandeReparation/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
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
