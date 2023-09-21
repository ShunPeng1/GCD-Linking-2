using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DebugNetworkUI : MonoBehaviour
{

    [SerializeField] Button _server, _host, _client;
    // Start is called before the first frame update
    void Awake()
    {
        _server.onClick.AddListener( () => NetworkManager.Singleton.StartServer());
        _host.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        _client.onClick.AddListener( () => NetworkManager.Singleton.StartClient());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
