using BattleCampusMatchServer.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class UserManager : MonoBehaviour
    {
        public static UserManager Instance;
        public User User { get; private set; } = new GuestUser();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }

            Destroy(this.gameObject);
        }
    }
}