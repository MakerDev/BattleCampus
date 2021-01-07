using BattleCampusMatchServer.Models.DTOs;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO : 이제 얘가 스스로 자기 정보만 동기화 하도록 해도 괜찮을 듯.
    public class MatchUI : MonoBehaviour
    {
        [SerializeField]
        private Text _matchNameText;
        [SerializeField]
        private Text _currentPlayerInfoText;
        [SerializeField]
        private Text _matchIDText;

        public void UpdateInfo(MatchDTO match)
        {
            _matchIDText.text = match.MatchID;
            _matchNameText.text = match.Name;
            _currentPlayerInfoText.text = $"{match.CurrentPlayers}/{match.MaxPlayers}";
        }
    }
}