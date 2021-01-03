using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PlayerSetup))]
    public class Player : NetworkBehaviour
    {
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

        private bool _isFirstSetup = true;

        public void PlayerSetUp()
        {
            if (isLocalPlayer)
            {
                GameManager.Instance.SetSceneCameraActive(false);
                GetComponent<PlayerSetup>().PlayerUIInstance.SetActive(true);
            }

            CmdBroadCastNewPlayerSetUp();
        }

        [Command]
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