using BattleCampusMatchServer.Models;
using BattleCampusMatchServer.Models.DTOs;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MatchMaking
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance = null;

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

        //This means the player has exited game or that the game is ended.
        //This is not synced as it's not a NetworkBehavior
        //private async void OnDisable()
        //{
        //    await NotifyPlayerExitAsync();
        //}

        public async Task NotifyPlayerExitAsync()
        {
            await MatchServer.Instance.NotifyPlayerExitAsync(IpPortInfo.IpAddress, Match.MatchID, UserManager.Instance.User);
        }
    }
}