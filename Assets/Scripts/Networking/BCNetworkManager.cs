using Assets.Scripts.MatchMaking;
using BattleCampusMatchServer.Models;
using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class BCNetworkManager : NetworkManager
    {
        private IpPortInfo ConfigureIpPortInfo()
        {
            var arguments = Environment.GetCommandLineArgs();

            var ipPortInfo = new IpPortInfo();

            for (int i = 0; i < arguments.Length; i++)
            {
                var arg = arguments[i];

                if (arg == "-ip")
                {
                    ipPortInfo.IpAddress = arguments[i + 1];
                }
                else if (arg == "-desktopPort")
                {
                    ipPortInfo.DesktopPort = int.Parse(arguments[i + 1]);
                }
                else if (arg == "-websocketPort")
                {
                    ipPortInfo.WebsocketPort = int.Parse(arguments[i + 1]);
                }
            }

            return ipPortInfo;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            var serverIpPortInfo = ConfigureIpPortInfo();

            Debug.Log("IP : " + serverIpPortInfo.IpAddress);

            var name = $"Server:{serverIpPortInfo.IpAddress}";

            MatchServer.Instance.RegisterServerAsync(name, serverIpPortInfo).ContinueWith(t =>
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
            //TODO : Make graceful stop.
            await MatchServer.Instance.UnRegisterServerAsync(networkAddress);

            base.OnStopServer();
        }
    }
}