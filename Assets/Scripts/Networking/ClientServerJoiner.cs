using Assets.Scripts.MatchMaking;
using BattleCampusMatchServer.Models;
using Mirror;
using Mirror.SimpleWeb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientServerJoiner : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;
    [SerializeField]
    private SimpleWebTransport _webTransport;
    [SerializeField]
    private TelepathyTransport _telepathyTransport;

    private void Start()
    {
        //TODO : Ip주소 받아와야 하니까 Auto Connect하면 안 될듯. 
        if (!Application.isBatchMode)
        {
            Debug.Log($"===Client Build===");
            var matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
            JoinServer(matchManager.IpPortInfo);
        }
        else
        {
            Debug.Log("===Server Build===");
        }
    }

    public void JoinServer(IpPortInfo ipPortInfo)
    {
        _networkManager.networkAddress = ipPortInfo.IpAddress;
        _telepathyTransport.port = (ushort)ipPortInfo.DesktopPort;
        _webTransport.port = (ushort)ipPortInfo.WebsocketPort;

        _networkManager.StartClient();
    }
}
