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
    /// <summary>
    /// Hub for Match API Server
    /// </summary>
    public class MatchServer
    {
        //private const string BASE_ADDRESS = "https://localhost:5001/api/";
        private const string BASE_ADDRESS = "https://battlecampusmatchserver.azurewebsites.net/api/";

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
            var userJson = JsonConvert.SerializeObject(UserManager.Instance.User);
            var userInfo = new StringContent(userJson, encoding: Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"matches/create?name={name}", userInfo);
            var matchCreationResultString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MatchCreationResultDTO>(matchCreationResultString);
        }

        public async Task<MatchJoinResultDTO> JoinMatch(string serverIp, string matchID, User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var userInfo = new StringContent(userJson, encoding: Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"matches/join?serverIp={serverIp}&matchID={matchID}", userInfo);
            var matchJoinResultString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MatchJoinResultDTO>(matchJoinResultString);
        }

        public async Task NotifyPlayerExitAsync(string serverIp, string matchID, User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var userInfo = new StringContent(userJson, encoding: Encoding.UTF8, "application/json");

            await _httpClient.PostAsync($"matches/notify/exit?serverIp={serverIp}&matchID={matchID}", userInfo);
        }

        public async Task<bool> RegisterServerAsync(string serverName, IpPortInfo ipPortInfo, int maxMatches = 5)
        {
            var jsonContent = JsonConvert.SerializeObject(ipPortInfo);
            var stringContent = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"server/register/{serverName}?maxMatches={maxMatches}", stringContent).ConfigureAwait(true);

            return response.IsSuccessStatusCode;
        }

        public async Task UnRegisterServerAsync(string ipAddress)
        {
            await _httpClient.DeleteAsync($"server/unregister/{ipAddress}");
            Debug.Log($"Unregister server {ipAddress}");
        }
    }
}
