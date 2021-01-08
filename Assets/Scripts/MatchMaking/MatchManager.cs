using BattleCampusMatchServer.Models;
using BattleCampusMatchServer.Models.DTOs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MatchMaking
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance { get; private set; }

        public MatchDTO Match { get; private set; }
        public IpPortInfo IpPortInfo { get; private set; } = new IpPortInfo();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }

            Destroy(this.gameObject);
        }

        public void ConfigureMatchInfo(MatchDTO match)
        {
            Match = match;
            IpPortInfo = match.IpPortInfo;
        }

        //TODO : turn this back on?
        //This means the player has exited game or that the game is ended.
        //This is not synced as it's not a NetworkBehavior
        //private async void OnDisable()
        //{
        //    await NotifyPlayerExitAsync();
        //}

        public async UniTask NotifyPlayerExitAsync()
        {
            await MatchServer.Instance.NotifyPlayerExitAsync(IpPortInfo.IpAddress, Match.MatchID, UserManager.Instance.User);
        }
    }
}