using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        public MatchSetting MatchSetting;

        [SerializeField]
        private GameObject _sceneCamera;
        [SerializeField]
        private GameObject _menuCanvas;
        [SerializeField]
        private InputField _renameInputField;

        [SerializeField]
        private GameObject _chatPanel;
        [SerializeField]
        private GameObject _textObjectPrefab;
        [SerializeField]
        private InputField _chatInputField;

        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public bool IsMenuOpen { get; private set; } = false;

        private const int MAX_MESSAGES = 10;
        private List<ChatMessage> _messages = new List<ChatMessage>();

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
            //TODO : find more intelligent way
            if (IsMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }


            if (_chatInputField.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    CmdPrintMessage(_chatInputField.text, Player.LocalPlayer.PlayerName);
                    _chatInputField.text = "";
                }
            }
            else
            {
                if (!_chatInputField.isFocused && Input.GetKeyDown(KeyCode.Return))
                {
                    _chatInputField.ActivateInputField();
                }
            }

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

        public void SetChatPanel(GameObject chatPanel)
        {
            _chatPanel = chatPanel;
        }

        [Command(ignoreAuthority = true)]
        public void CmdPrintMessage(string message, string sender)
        {
            RpcPrintMessage(message, sender);
        }

        [ClientRpc]
        public void RpcPrintMessage(string message, string sender)
        {
            PrintMessage(message, sender);
        }

        public void PrintMessage(string message, string sender)
        {
            if (_messages.Count >= MAX_MESSAGES)
            {
                Destroy(_messages[0].TextObject.gameObject);
                _messages.RemoveAt(0);
            }

            var chatMessage = new ChatMessage();
            chatMessage.Message = message;
            chatMessage.Sender = sender;
            chatMessage.TextObject = Instantiate(_textObjectPrefab, _chatPanel.transform).GetComponent<Text>();

            var hasSender = string.IsNullOrEmpty(sender);

            chatMessage.ChatType = hasSender ? ChatType.Info : ChatType.Player;
            chatMessage.TextObject.text = hasSender ? message : $"{sender}: {message}";
            chatMessage.TextObject.color = GetMessageColor(chatMessage.ChatType);

            _messages.Add(chatMessage);
        }

        private Color GetMessageColor(ChatType chatType)
        {
            switch (chatType)
            {
                case ChatType.Info:
                    return Color.red;

                case ChatType.Player:
                    return Color.black;

                default:
                    break;
            }

            return Color.black;
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