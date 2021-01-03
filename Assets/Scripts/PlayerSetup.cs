using Assets.Scripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerSetup : NetworkBehaviour
    {
        private const string DONT_DRAW_LAYER_NAME = "DontDraw";

        [HideInInspector]
        public GameObject PlayerUIInstance;

        [SerializeField]
        private Behaviour[] _componentsToDiable;
        [SerializeField]
        private string _remoteLayerName = "RemotePlayer";

        [SerializeField]
        private GameObject _playerUIPrefab;
        [SerializeField]
        private GameObject _playerGraphics;

        public override void OnStartClient()
        {
            base.OnStartClient();

            string netId = GetComponent<NetworkIdentity>().netId.ToString();
            Player player = GetComponent<Player>();

            GameManager.RegisterPlayer(netId, player);
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                foreach (var component in _componentsToDiable)
                {
                    component.enabled = false;
                }

                gameObject.layer = LayerMask.NameToLayer(_remoteLayerName);
            }
            else
            {
                PlayerUIInstance = Instantiate(_playerUIPrefab);
                PlayerUIInstance.name = _playerUIPrefab.name;

                Utils.SetLayerRecursive(_playerGraphics, LayerMask.NameToLayer(DONT_DRAW_LAYER_NAME));

                //Configure PlayerUI
                PlayerUI playerUI = PlayerUIInstance.GetComponent<PlayerUI>();
                if (playerUI == null)
                {
                    Debug.LogError("No PlayerUI Component");
                }

                playerUI.SetController(GetComponent<PlayerController>());
                GetComponent<Player>().PlayerSetUp();
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Destroy(PlayerUIInstance);

            if (isLocalPlayer)
            {
                GameManager.Instance.SetSceneCameraActive(true);
            }

            GameManager.UnRegisterPlayer(transform.name);
        }
    }

}
