using Assets.Scripts.MatchMaking;
using BattleCampusMatchServer.Models;
using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class BCNetworkManager : NetworkManager
    {
        public override void OnStartServer()
        {
            base.OnStartServer();

            //TOOD : Send server open api call to the match server
            var serverInfo = new IpPortInfo
            {
                IpAddress = networkAddress,
                DesktopPort = 7777,
                WebsocketPort = 7778,
            };
            
            var name = $"Server:{networkAddress}";

            MatchServer.Instance.RegisterServerAsync(name, serverInfo).ContinueWith (t =>
            {
                if (t.Result == false)
                {
                    //TODO : Shutdown server instance and report it.
                    //Shutdown();
                }
            });
        }

        public override async void OnStopServer()
        {
            base.OnStopServer();

            //TODO : Make graceful stop.
            await MatchServer.Instance.UnRegisterServerAsync(networkAddress);
        }
    }
}