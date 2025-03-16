using FicheReparation.Entity;
using FicheReparation.Models;
using Microsoft.AspNetCore.Mvc;

namespace FicheReparation.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // Lister tous les clients
        public async Task<IActionResult> Liste()
        {
            var clients = await _clientRepository.GetAllClientsAsync();
            return View(clients);
        }

        //  Afficher les détails d'un client
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        //  Afficher le formulaire de création
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //  Traiter la création d'un client
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                await _clientRepository.AddClientAsync(client);
                return RedirectToAction("Liste", "Client");
            }
            return View(client);
        }

        //  Afficher le formulaire d'édition
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        //  Traiter la modification d'un client
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _clientRepository.UpdateClientAsync(client);
                return RedirectToAction("Liste", "Client");
            }
            return View(client);
        }

        //  Afficher le formulaire de suppression
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        //  Traiter la suppression d'un client
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientRepository.DeleteClientAsync(id);
            return RedirectToAction("Liste", "Client");
        }
    }
}
