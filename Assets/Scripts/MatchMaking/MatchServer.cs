using BattleCampusMatchServer.Models;
using BattleCampusMatchServer.Models.DTOs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

namespace Assets.Scripts.MatchMaking
{
    /// <summary>
    /// Hub for Match API Server
    /// </summary>
    public class MatchServer
    {
        private const string BASE_ADDRESS = "https://localhost:5001/api/";
        //private const string BASE_ADDRESS = "https://battlecampusmatchserver.azurewebsites.net/api/";

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

        public async UniTask<List<MatchDTO>> GetAllMatchesAsync()
        {
            var result = await UnityWebRequest.Get(BASE_ADDRESS + "matches").SendWebRequest();

            if (result.isHttpError || result.isNetworkError)
            {
                return null;
            }

            Debug.Log(result.downloadHandler.text);

            return JsonConvert.DeserializeObject<List<MatchDTO>>(result.downloadHandler.text);
        }

        public async UniTask<MatchCreationResultDTO> CreateMatchAsync(string name)
        {
            var userJson = JsonConvert.SerializeObject(UserManager.Instance.User);
            var request = UnityWebRequest.Post($"{BASE_ADDRESS}matches/create?name={name}", "");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(userJson));
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");

            var response = await request.SendWebRequest();
            var matchCreationResultString = response.downloadHandler.text;

            return JsonConvert.DeserializeObject<MatchCreationResultDTO>(matchCreationResultString);
        }

        public async UniTask<MatchJoinResultDTO> JoinMatchAsync(string serverIp, string matchID, User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var request = UnityWebRequest.Post($"{BASE_ADDRESS}matches/join?serverIp={serverIp}&matchID={matchID}", "");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(userJson));
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");

            var response = await request.SendWebRequest();
            var matchJoinResultString = response.downloadHandler.text;

            return JsonConvert.DeserializeObject<MatchJoinResultDTO>(matchJoinResultString);
        }

        public async UniTask NotifyPlayerExitAsync(string serverIp, string matchID, User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var request = UnityWebRequest.Post($"{BASE_ADDRESS}matches/notify/exit?serverIp={serverIp}&matchID={matchID}", "");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(userJson));
            request.uploadHandler.contentType = "application/json";

            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();
        }

        public async UniTask<bool> RegisterServerAsync(string serverName, IpPortInfo ipPortInfo, int maxMatches = 5)
        {
            var jsonContent = JsonConvert.SerializeObject(ipPortInfo);
            var request = UnityWebRequest.Post($"{BASE_ADDRESS}server/register/{serverName}?maxMatches={maxMatches}", "");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");

            var response = await request.SendWebRequest();

            return (!response.isNetworkError && !response.isHttpError);
        }

        public async UniTask UnRegisterServerAsync(string ipAddress)
        {
            var request = UnityWebRequest.Delete($"{BASE_ADDRESS}server/unregister/{ipAddress}");
            await request.SendWebRequest();
            Debug.Log($"Unregister server {ipAddress}");
        }
    }
}
