using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PlayerSetup))]
    public class Player : NetworkBehaviour
    {
        public static Player LocalPlayer { get; private set; }

        [SyncVar]
        private bool _isDead = false;
        public bool IsDead
        {
            get { return _isDead = false; }
            protected set { _isDead = value; }
        }

        [SerializeField]
        private float _maxHealth = 100;

        [SyncVar]
        private float _currentHealth;

        [SerializeField]
        private Behaviour[] _disableOnDeath;
        [SerializeField]
        private GameObject[] _disableGameObjectsOnDeath;

        private bool[] _wasEnabled;

        [SerializeField]
        private GameObject _deatchEffect;
        [SerializeField]
        private GameObject _spawnEffect;

       
        [SerializeField]
        private PlayerInfoUI _playerInfo;

        [SyncVar(hook = nameof(OnNameSet))]
        private string _playerName = $"player";

        public string PlayerName { get { return _playerName; } }

        private bool _isFirstSetup = true;

        private void Start()
        {
            _currentHealth = _maxHealth;

            string netId = GetComponent<NetworkIdentity>().netId.ToString();
            _playerName = $"Player{netId}";

            if (isLocalPlayer)
            {
                LocalPlayer = this;
            }
        }
      
        public void OnNameSet(string old, string newName)
        {
            if (isLocalPlayer)
            {
                //HACK
                PlayerSetup.PlayerUI?.SetLocalPlayerName(newName);
            }

            _playerInfo.SetPlayer(this);
        }

        public void SetName(string newName)
        {
            CmdChangePlayerName(newName);
        }

        [Command]
        public void CmdChangePlayerName(string newName)
        {
            _playerName = newName;
        }        

        public void PlayerSetUp()
        {
            if (isLocalPlayer)
            {
                GameManager.Instance.SetSceneCameraActive(false);
                GetComponent<PlayerSetup>().PlayerUIInstance.SetActive(true);
            }

            Debug.Log($"Player Setup is called on Player{netId}");
            CmdBroadCastNewPlayerSetUp();
        }

        [Command(ignoreAuthority = true)]
        private void CmdBroadCastNewPlayerSetUp()
        {
            RpcSetupPlayeronAllClients();
        }

        [ClientRpc]
        private void RpcSetupPlayeronAllClients()
        {
            if (_isFirstSetup)
            {
                _wasEnabled = new bool[_disableOnDeath.Length];

                for (int i = 0; i < _disableOnDeath.Length; i++)
                {
                    _wasEnabled[i] = _disableOnDeath[i].enabled;
                }


                _isFirstSetup = false; ;
            }

            SetDefaults();
        }

        public void SetDefaults()
        {
            string caller = isServer ? "Server" : "Client";
            Debug.Log($"{caller} called SetDefaults for {PlayerName}");

            IsDead = false;

            _currentHealth = _maxHealth;

            for (int i = 0; i < _disableOnDeath.Length; i++)
            {
                _disableOnDeath[i].enabled = _wasEnabled[i];
            }

            for (int i = 0; i < _disableGameObjectsOnDeath.Length; i++)
            {
                _disableGameObjectsOnDeath[i].SetActive(true);
            }

            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        [ClientRpc]
        public void RpcTakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }

            _currentHealth -= damage;

            Debug.Log(transform.name + " now has " + _currentHealth + " health.");

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public float GetCurrentHpRatio()
        {
            return _currentHealth / _maxHealth;
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(GameManager.Instance.MatchSetting.RespawnTime);

            var spawnPoints = NetworkManager.singleton.GetStartPosition();
            transform.position = spawnPoints.position;
            transform.rotation = spawnPoints.rotation;

            yield return new WaitForSeconds(0.1f);

            PlayerSetUp();

            Debug.Log(transform.name + " respawned");
        }

        private void Die()
        {
            IsDead = true;

            //Disable components
            for (int i = 0; i < _disableOnDeath.Length; i++)
            {
                _disableOnDeath[i].enabled = false;
            }

            for (int i = 0; i < _disableGameObjectsOnDeath.Length; i++)
            {
                _disableGameObjectsOnDeath[i].SetActive(false);
            }

            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            GameObject effectObject = Instantiate(_deatchEffect, transform.position, Quaternion.identity);
            Destroy(effectObject, 1.5f);

            Debug.Log(transform.name + " is dead!");

            if (isLocalPlayer)
            {
                GameManager.Instance.SetSceneCameraActive(true);
                GetComponent<PlayerSetup>().PlayerUIInstance.SetActive(false);
            }

            //Respawn
            StartCoroutine(Respawn());
        }
    }
}