using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;  // Ajouté pour accéder aux variables d'environnement
using DotNetEnv;  // Ajouté pour charger le fichier .env

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        // Charger les variables d'environnement
        public PlayerController()
        {
            Env.Load(); // Charge le fichier .env automatiquement
        }

        // La clé API est récupérée depuis les variables d'environnement
        private readonly string API_KEY = Environment.GetEnvironmentVariable("PALADIUM_API_KEY");

        [HttpGet("{playerName}")]
        public async Task<IActionResult> GetPlayerInfo2(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return BadRequest("Veuillez entrer un pseudo de joueur.");
            }

            try
            {
                string uuid = await GetUUIDFromPlayerName(playerName);
                string playerInfo = await GetPlayerInfo(uuid);
                string friendsList = await GetPlayerFriendList(uuid);
                string playerJob = await GetPlayerJobs(uuid);
                string playerClicker = await GetPlayerClicker(uuid);

                var result = new
                {
                    playerInfo,
                    friendsList,
                    playerJob,
                    playerClicker
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur : {ex.Message}");
            }
        }

        private async Task<string> GetUUIDFromPlayerName(string playerName)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
                string API_URL = $"https://api.paladium.games/v1/paladium/player/profile/{playerName}";

                HttpResponseMessage response = await client.GetAsync(API_URL);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JObject.Parse(jsonResponse);
                    return jsonDoc["uuid"].ToString();
                }
                else
                {
                    throw new Exception($"Erreur : {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        private async Task<string> GetPlayerInfo(string uuid)
        {
            return await CallApi($"https://api.paladium.games/v1/paladium/player/profile/{uuid}");
        }

        private async Task<string> GetPlayerFriendList(string uuid)
        {
            return await CallApi($"https://api.paladium.games/v1/paladium/player/profile/{uuid}/friends");
        }

        private async Task<string> GetPlayerJobs(string uuid)
        {
            return await CallApi($"https://api.paladium.games/v1/paladium/player/profile/{uuid}/jobs");
        }

        private async Task<string> GetPlayerClicker(string uuid)
        {
            return await CallApi($"https://api.paladium.games/v1/paladium/player/profile/{uuid}/clicker");
        }

        private async Task<string> CallApi(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Erreur : {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
    }
}
