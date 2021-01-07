using BattleCampusMatchServer.Models;
using BattleCampusMatchServer.Models.DTOs;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.MatchMaking
{
    public class MatchManager : MonoBehaviour
    {
        public IpPortInfo IpPortInfo { get; private set; } = new IpPortInfo();

        void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void ConfigureMatchInfo(MatchDTO match)
        {
            IpPortInfo = match.IpPortInfo;
        }
    }
}