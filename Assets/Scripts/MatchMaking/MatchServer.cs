using BattleCampusMatchServer.Models;
using BattleCampusMatchServer.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VR;

namespace Assets.Scripts.MatchMaking
{
    public class MatchServer
    {
        private const string BASE_ADDRESS = "https://localhost:5001/api/";

        private HttpClient _httpClient;

        private static MatchServer _instance = null;
        public static MatchServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MatchServer();
                }
                return _instance;
            }
        }

        public MatchServer()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<MatchDTO>> GetAllMatchesAsync()
        {
            var response = await _httpClient.GetAsync("matches");

            if (response.IsSuccessStatusCode)
            {

                var result = JsonConvert.DeserializeObject<List<MatchDTO>>(await response.Content.ReadAsStringAsync());
                return result;
            }

            return null;
        }

        public async Task<MatchCreationResultDTO> CreateMatchAsync(string name)
        {
            var result = await _httpClient.PostAsync($"matches/create?name={name}", null);

            var matchCreationResultString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MatchCreationResultDTO>(matchCreationResultString);
        }

        public Task NotifyPlayerJoinAsync(string ip, string matchID, string playerId)
        {
            throw new NotImplementedException();
        }

        public Task NotifyPlayerExitAsync(string ip, string matchID, string playerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterServerAsync(string serverName, IpPortInfo ipPortInfo, int maxMatches = 5)
        {
            var jsonContent = JsonConvert.SerializeObject(ipPortInfo);
            var stringContent = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"server/register/{serverName}?maxMatches={maxMatches}", stringContent).ConfigureAwait(true);

            return response.IsSuccessStatusCode;
        }

        public async Task UnRegisterServerAsync(string ipAdrress)
        {
            await _httpClient.DeleteAsync($"server/unregister/{ipAdrress}");
            Debug.Log($"Unregister server {ipAdrress}");
        }
    }
}
