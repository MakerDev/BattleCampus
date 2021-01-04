using FirstGearGames.Utilities.Objects;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _thrusterFuelFill;

        [SerializeField]
        private RectTransform _hpBar;

        [SerializeField]
        private Text _playerNameText;

        private PlayerController _controller;
        private Player _player;

        private void Update()
        {
            SetFuelAmout(_controller.GetThrusterFuelAmount());
            _hpBar.SetScale(new Vector3(_player.GetCurrentHpRatio(), 1, 1));
        }

        public void SetLocalPlayerName(string name)
        {
            _playerNameText.text = name;
        }

        public void SetController(PlayerController playerController)
        {
            _controller = playerController;
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        void SetFuelAmout(float amount)
        {
            _thrusterFuelFill.localScale = new Vector3(1, amount, 1);
        }
    }
}