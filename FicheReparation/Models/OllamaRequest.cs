using System.Text.Json.Serialization;
namespace FicheReparation.Models
{
    public class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama3";

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = "";

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        [JsonPropertyName("options")]
        public Dictionary<string, object>? Options { get; set; }
    }
}
