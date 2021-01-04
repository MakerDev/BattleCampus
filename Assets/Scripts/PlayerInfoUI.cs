using FirstGearGames.Utilities.Objects;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerInfoUI : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private RectTransform _hpBar;

        private Player _player;

        public void SetPlayer(Player player)
        {
            _player = player;

            _playerNameText.text = player.PlayerName;
        }

        // Update is called once per frame
        void Update()
        {
            if (_player == null)
            {
                return;
            }

            _hpBar.SetScale(new Vector3(_player.GetCurrentHpRatio(), 1, 1));
        }
    }
}