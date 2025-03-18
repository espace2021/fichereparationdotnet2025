using Microsoft.AspNetCore.Mvc;
using FicheReparation.Helpers;
using FicheReparation.Models;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FicheReparation.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly HttpClient _httpClient;

        public ChatController(IConfiguration configuration, HttpClient httpClient)
        {
            // Injecter IConfiguration dans le constructeur du contrôleur
            _chatService = new ChatService(configuration);  // Passer IConfiguration à ChatService
            // http
            _httpClient = httpClient;
        }

        // Cette méthode sert à afficher le formulaire de question
        [HttpGet]
        public IActionResult Chat()
        {
            return View(); // Cela rendra la vue "Chat.cshtml"
        }

        [HttpPost]
        public async Task<IActionResult> PoserQuestion(string userQuestion)
        {
            if (string.IsNullOrEmpty(userQuestion))
            {
                ViewData["ResponseMessage"] = "La question est requise.";
                return View("Chat");
            }

            var analysis = await AnalyzeIntent(userQuestion);

            if (string.IsNullOrEmpty(analysis?.Query))
            {
                ViewData["ResponseMessage"] = $"Impossible de générer une requête SQL. Analyse de la question : {userQuestion}";
                return View("Chat");
            }
            
            Console.WriteLine($"SQL Query: {analysis.Query}");  // Add logging here for the generated query
            var data = await _chatService.ExecuteSqlQuery(analysis);
           
            ViewData["ResponseMessage"] = GenerateResponse(data);

            return View("Chat");
        }

        private async Task<IntentAnalysisResult> AnalyzeIntent(string userQuestion)
        {
            var prompt = $@"
        Tu es un assistant spécialisé en bases de données SQL Server. 
        Génère une requête SQL Server valide et optimisée pour répondre à la question suivante :
        '{userQuestion}'
        
        - Assure-toi que la syntaxe est correcte et compatible avec SQL Server.
        - Utilise des alias explicites pour améliorer la lisibilité si nécessaire.
        - Évite les requêtes dangereuses comme DROP, DELETE sans condition, ou toute modification irréversible des données.
        - Si la question manque de précision, propose une requête générique en supposant des noms de tables courants.
        - Les noms des tables en Français
        - Les noms des champs en un mot en Français
        - Retourne uniquement la requête SQL, sans explication supplémentaire.
        - La requête se termine toujours par un point virgule
    ";

            var llamaResponse = await CallLlama(prompt);

            Console.WriteLine($"Réponse brute de Llama : {llamaResponse}");

            if (string.IsNullOrEmpty(llamaResponse))
            {
                return new IntentAnalysisResult { Query = "Impossible de générer une requête SQL.", Table = string.Empty };
            }

            // Enregistrer la réponse brute pour débogage
            Console.WriteLine($"Réponse brute de Llama : {llamaResponse}");

            var result = new IntentAnalysisResult();

            try
            {
                // Vérification si la réponse contient une requête SQL valide
                var queryMatch = Regex.Match(llamaResponse, @"SELECT.*FROM.*", RegexOptions.IgnoreCase);
                if (queryMatch.Success)
                {
                   
                    // Désérialiser la réponse JSON
                    dynamic responseObject = JsonConvert.DeserializeObject(llamaResponse);

                    // Extraire la chaîne contenant la requête SQL
                    string responseText = responseObject.response;

                    // Extraire la première occurrence de la requête SELECT
                    string sqlQuery = ExtractSqlQuery(responseText);

                    // Afficher la requête
                    Console.WriteLine("Première occurrence de la requête SQL :");
                    Console.WriteLine(sqlQuery);
                         
                    result.Table = "clients";
                    result.Query = sqlQuery;
                }
                else
                {
                    result.Query = "Impossible de générer une requête SQL valide.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'analyse de la réponse : {ex.Message}");
                result.Query = "Erreur lors de l'analyse de la réponse.";
            }

            return result;
        }


        // Fonction pour extraire la première requête SQL (par exemple, "SELECT * FROM Client;")
        public static string ExtractSqlQuery(string text)
        {
            // Recherche de l'index où commence la requête "SELECT"
            int startIndex = text.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);

            // Recherche de l'index où se termine la requête, marqué par le point-virgule ";"
            int endIndex = text.IndexOf(";", startIndex);

            // Si les deux indices sont trouvés, extraire la sous-chaîne
            if (startIndex != -1 && endIndex != -1)
            {
                // Extraire la requête SQL de l'index où "SELECT" commence jusqu'à la fin du point-virgule
                return text.Substring(startIndex, endIndex - startIndex + 1);
            }

            // Si la requête SQL n'est pas trouvée, retourner null
            return null;
        }


        private string GenerateResponse(List<Dictionary<string, object>> data)
        {
            if (data.Count == 0)
                return "Aucune donnée trouvée.";

            var response = "<ul>";
            foreach (var row in data)
            {
                response += "<li>" + string.Join(", ", row.Select(kv => $"{kv.Key}: {kv.Value}")) + "</li>";
            }
            response += "</ul>";

            return response;
        }

        public async Task<string> CallLlama(string prompt)
        {
            // Inclure le modèle "llama3" dans la requête
            var requestContent = new OllamaRequest
            {
                Model = "llama3",
                Prompt = prompt,
                Stream = false
            };

            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(requestContent);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Envoi de la requête à l'API Ollama
            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ollama Response: {response.Content}");
                return responseContent;
            }

            // Si la réponse n'est pas OK, afficher le code de statut
            Console.WriteLine($"Ollama Response Pb : {response.StatusCode} - {response.ReasonPhrase}");
            return "Error contacting Ollama API";
        }



    }
}
