using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public MatchSetting MatchSetting;

        [SerializeField]
        private GameObject _sceneCamera;
        [SerializeField]
        private GameObject _menuCanvas;
        [SerializeField]
        private InputField _renameInputField;

        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public bool IsMenuOpen { get; private set; } = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multiple GameManagers in a scene");
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _menuCanvas.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (_menuCanvas.activeSelf)
                {
                    Resume();
                }
                else
                {
                    OpenMenu();
                }
            }
        }

        public void SetSceneCameraActive(bool isActive)
        {
            if (_sceneCamera == null)
            {
                return;
            }

            _sceneCamera.SetActive(isActive);
        }

        public void OpenMenu()
        {
            if (_menuCanvas != null)
            {
                _menuCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                IsMenuOpen = true;
            }
        }

        public void Resume()
        {
            if (_menuCanvas != null)
            {
                _menuCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                IsMenuOpen = false;
            }
        }

        public void RenameLocalPlayer()
        {
            Player.LocalPlayer.SetName(_renameInputField.text);
        }

        #region PLAYER TRACKING
        public const string PLAYER_ID_PREFIX = "Player";

        /// <summary>
        /// Key : PlayerId == transform.name
        /// </summary>
        public static void RegisterPlayer(string netId, Player player)
        {
            string playerId = PLAYER_ID_PREFIX + netId;
            _players.Add(playerId, player);
            player.transform.name = playerId;

            Debug.Log($"Client : {playerId} is registered");
        }

        public static Player GetPlayer(string playerId)
        {
            if (_players.ContainsKey(playerId))
            {
                return _players[playerId];
            }

            throw new System.Exception($"No player with ID {playerId} is found");
        }

        public static void UnRegisterPlayer(string playerId)
        {
            _players.Remove(playerId);
        }

        //private void OnGUI()
        //{
        //    GUILayout.BeginArea(new Rect(200, 200, 200, 200));
        //    GUILayout.BeginVertical();

        //    foreach (var playerId in _players.Keys)
        //    {
        //        GUILayout.Label(playerId + " - " + _players[playerId].transform.name);
        //    }

        //    GUILayout.EndVertical();
        //    GUILayout.EndArea();
        //}
        #endregion
    }
}