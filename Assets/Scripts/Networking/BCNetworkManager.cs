using Assets.Scripts.MatchMaking;
using BattleCampusMatchServer.Models;
using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class BCNetworkManager : NetworkManager
    {
        public override void OnStartServer()
        {
            base.OnStartServer();

            networkAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First((x) =>
                {
                    return x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
                }).ToString();

            //TOOD : Send server open api call to the match server
            var serverInfo = new IpPortInfo
            {
                IpAddress = networkAddress,
                DesktopPort = 7777,
                WebsocketPort = 7778,
            };

            Debug.Log("IP : " + networkAddress);

            var name = $"Server:{networkAddress}";

            MatchServer.Instance.RegisterServerAsync(name, serverInfo).ContinueWith(t =>
           {
               if (t.Result == false)
               {
                    //TODO : Shutdown server instance and report it.
                    //Shutdown();
                }
           });
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            Debug.LogError("Disconnect");
        }

        public override async void OnStopServer()
        {
            //TODO : Make graceful stop.
            await MatchServer.Instance.UnRegisterServerAsync(networkAddress);

            base.OnStopServer();
        }
    }
}