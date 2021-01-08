using Assets.Scripts.MatchMaking;
using BattleCampusMatchServer.Models.DTOs;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// 룸 매니저의 역할은, 유저가 룸(매치)를 Host or Join하게 하여
    /// 적절한 MatchGuid를 부여받고 해당룸으로 이동하게 하는 것이다. 
    /// 게임씬(또는 게임 Ready 씬)부터는 GameManager 같은 애들이 handle하는 것이고, 씬이 전환될때, RoomManager는 굳이 필요가 없다.
    /// 얘는 그냥 서버에만 있고, LobbyPlayer가 Create랑 Join을 호출하게만 하면 될 거 같은데..?
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        [SerializeField]
        private GameObject _matchUIPrefab;
        [SerializeField]
        private GameObject _matchUIPanel;

        private List<GameObject> _matchUIInstances = new List<GameObject>();

        [SerializeField]
        private Canvas _createMatchUI;
        [SerializeField]
        private Button _createMatchButton;
        [SerializeField]
        private MatchManager _matchManager;

        [Header("Debug")]
        [SerializeField]
        private Text _matchCreateResultText;

        private List<GameObject> _matchUIs = new List<GameObject>();

        private void Start()
        {
            Instance = this;

            StartCoroutine(Run());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator Run()
        {
            FetchAllMatchesAsync();

            yield return new WaitForSecondsRealtime(2.5f);
        }

        public async void RefreshLobby()
        {
            await FetchAllMatchesAsync().ConfigureAwait(true);
        }

        public async Task FetchAllMatchesAsync()
        {
            var matches = await MatchServer.Instance.GetAllMatchesAsync();

            if (matches == null)
            {
                return;
            }

            foreach (var matchUIInstance in _matchUIInstances)
            {
                Destroy(matchUIInstance);
            }

            _matchUIInstances.Clear();
            _matchUIs.Clear();

            foreach (var matchDto in matches)
            {
                AddMatch(matchDto);
            }
        }

        private void AddMatch(MatchDTO match)
        {
            var matchUIInstance = Instantiate(_matchUIPrefab, _matchUIPanel.transform);
            var matchUI = matchUIInstance.GetComponent<MatchUI>();

            _matchUIInstances.Add(matchUIInstance);
            matchUI.UpdateInfo(match);

            _matchUIs.Add(matchUIInstance);
        }

        public void CreateNewMatch()
        {
            _createMatchButton.enabled = false;

            CreateNewMatchAsync().ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                _createMatchButton.enabled = true;
            });
        }

        public void MoveToMatch(MatchDTO match)
        {
            //Configure MatchManager
            //_matchManager.ConfigureMatchInfo(match);
            MatchManager.Instance.ConfigureMatchInfo(match);
            //TODO : Move to  GameScene 
            SceneManager.LoadScene("GameScene");
        }

        public async Task CreateNewMatchAsync()
        {
            var result = await MatchServer.Instance.CreateMatchAsync("Room1");

            var creationResultText = $"{result.IsCreationSuccess} : {result.Match.MatchID}";
            _matchCreateResultText.text = creationResultText;

            if (result.IsCreationSuccess)
            {
                MoveToMatch(result.Match);
                UserManager.Instance.User.IsHost = true;
            }
        }
    }
}