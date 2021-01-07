using Assets.Scripts.MatchMaking;
using Assets.Scripts.Networking;
using BattleCampusMatchServer.Models.DTOs;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO : 이제 얘가 스스로 자기 정보만 동기화 하도록 해도 괜찮을 듯.
    public class MatchUI : MonoBehaviour
    {
        private MatchDTO _match;

        [SerializeField]
        private Text _matchNameText;
        [SerializeField]
        private Text _currentPlayerInfoText;
        [SerializeField]
        private Text _matchIDText;
        [SerializeField]
        private Button _joinButton;

        public void UpdateInfo(MatchDTO match)
        {
            _match = match;

            _matchIDText.text = match.MatchID;
            _matchNameText.text = match.Name;
            _currentPlayerInfoText.text = $"{match.CurrentPlayersCount}/{match.MaxPlayers}";
        }

        public void JoinMatch()
        {
            _joinButton.enabled = false;

            JoinMatchAsync().ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                _joinButton.enabled = true;
            });
        }

        public async Task JoinMatchAsync()
        {
            var result = await MatchServer.Instance.JoinMatch(_match.IpPortInfo.IpAddress, _match.MatchID, UserManager.Instance.User);

            if (result.JoinSucceeded == false)
            {
                //TODO : prompt joining fail.
                Debug.LogError(result.JoinFailReason);
                return;
            }

            LobbyManager.Instance.MoveToMatch(result.Match);
        }
    }
}