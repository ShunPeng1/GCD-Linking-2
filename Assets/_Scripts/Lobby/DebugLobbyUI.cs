using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugLobbyUI : MonoBehaviour
{
    
    [SerializeField] private Button _createLobby;
    [SerializeField] private Button _joinLobby;
    
    // Start is called before the first frame update
    void Start()
    {
        _createLobby.onClick.AddListener( () => MultiplayerManager.Instance.CreateLobby());
        _joinLobby.onClick.AddListener( () => MultiplayerManager.Instance.QuickJoinLobby());
    }

}
