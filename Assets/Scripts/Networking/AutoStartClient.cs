using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartClient : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;

    //TODO : 서버 ip는 매치 서버에 요청해서 가져오기
    private void Start()
    {
        if (!Application.isBatchMode)
        {
            Debug.Log($"===Client Build===");
            _networkManager.StartClient();
        }
        else
        {
            Debug.Log("===Server Build===");
        }
    }

    public void JoinLocal()
    {
        _networkManager.networkAddress = "localhost";
        _networkManager.StartClient();
    }
}
