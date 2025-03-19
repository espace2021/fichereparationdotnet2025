using FicheReparation.Models;
using Microsoft.Data.SqlClient;

namespace FicheReparation.Helpers
{
    public class ChatService
    {
        private readonly string _connectionString;

        public ChatService(IConfiguration configuration)
        {
            // Récupérer la chaîne de connexion depuis appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<List<Dictionary<string, object>>> ExecuteSqlQuery(IntentAnalysisResult analysis)
        {
          
            if (string.IsNullOrEmpty(analysis?.Table) || string.IsNullOrEmpty(analysis?.Query))
                return new List<Dictionary<string, object>>();

            var results = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(analysis.Query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            return results;
        }

    }
}
