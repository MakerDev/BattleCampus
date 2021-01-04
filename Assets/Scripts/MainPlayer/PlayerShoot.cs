using Assets.Scripts;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour
    {
        private const string PLAYER_TAG = "Player";

        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _layerMask;

        private WeaponManager _weaponManager;
        private PlayerWeapon _currentWeapon;

        private void Start()
        {
            if (_camera == null)
            {
                Debug.LogError("PlayerShoot : No camera reference");
                this.enabled = false;
            }
            _weaponManager = GetComponent<WeaponManager>();
        }

        private void Update()
        {
            _currentWeapon = _weaponManager.GetCurrentWeapon();

            if (!isLocalPlayer)
            {
                return;
            }

            if (_currentWeapon.FireRate <= 0f)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    InvokeRepeating(nameof(Shoot), 0f, 1f / _currentWeapon.FireRate);
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    CancelInvoke("Shoot");
                }
            }
        }

        [Command]
        private void CmdOnHit(Vector3 position, Vector3 normal)
        {
            RpcDoHitEffect(position, normal);
        }

        [ClientRpc]
        private void RpcDoHitEffect(Vector3 position, Vector3 normal)
        {
            GameObject hitEffect = Instantiate(_weaponManager.GetCurrentWeaponGraphics().HitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(hitEffect, 1.5f);
        }

        [Command]
        private void CmdOnShoot()
        {
            RpcDoShootEffect();
        }

        [ClientRpc]
        private void RpcDoShootEffect()
        {
            _weaponManager.GetCurrentWeaponGraphics().MuzzleFlash.Play();
        }


        [Client]
        private void Shoot()
        {
            if (!isLocalPlayer)
            {
                return;
            }


            CmdOnShoot();

            RaycastHit raycastHit;
            Debug.Log($"Shoot!");

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out raycastHit, _currentWeapon.Range, _layerMask))
            {
                //We hit something
                Debug.Log($"Hit : {raycastHit.collider.name}");

                if (raycastHit.collider.CompareTag(PLAYER_TAG))
                {
                    CmdPlayershot(raycastHit.collider.name, _currentWeapon.Damage);
                }

                CmdOnHit(raycastHit.point, raycastHit.normal);                
            }
        }

        [Command]
        void CmdPlayershot(string playerId, float damage)
        {
            Debug.Log(playerId + " has been shot by damage " + damage);

            Player player = GameManager.GetPlayer(playerId);

            player.RpcTakeDamage(damage);
        }
    }
}