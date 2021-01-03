using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _thrusterFuelFill;

        private PlayerController _controller;

        private void Update()
        {
            SetFuelAmout(_controller.GetThrusterFuelAmount());
        }

        public void SetController(PlayerController playerController)
        {
            _controller = playerController;
        }

        void SetFuelAmout(float amount)
        {
            _thrusterFuelFill.localScale = new Vector3(1, amount, 1);
        }
    }
}