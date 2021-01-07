using BattleCampusMatchServer.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class UserManager : MonoBehaviour
    {
        public static UserManager Instacne;
        public User User { get; private set; } = new GuestUser();

        private void Start()
        {
            Instacne = this;

            DontDestroyOnLoad(this);
        }
    }
}