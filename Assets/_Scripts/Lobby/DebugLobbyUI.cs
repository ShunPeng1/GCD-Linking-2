using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


public class DebugLobbyUI : MonoBehaviour
{
    
    [SerializeField] private Button _createLobby;
    [SerializeField] private Button _joinLobby;
    [SerializeField] private Button _refreshLobby;

    [Header("Lobby List")] 
    [SerializeField] private RectTransform _lobbyContent;
    [SerializeField] private LobbyPanel _lobbyPanelPrefab;
    private List<LobbyPanel> _lobbyPanels = new List<LobbyPanel>();

    // Start is called before the first frame update
    void Start()
    {
        _createLobby.onClick.AddListener( () => MultiplayerManager.Instance.CreateLobby());
        _joinLobby.onClick.AddListener( () => MultiplayerManager.Instance.QuickJoinLobby());
        _refreshLobby.onClick.AddListener(RefreshLobby);
        
    }

    private async void RefreshLobby()
    {
        QueryResponse queryResponse = await MultiplayerManager.Instance.QueryLobbiesAsync();
        
        ClearLobbyPanels();
        
        foreach (var lobby in queryResponse.Results)
        {
        
            var lobbyPanel = Instantiate(_lobbyPanelPrefab, _lobbyContent);
            lobbyPanel.Build(lobby);
            _lobbyPanels.Add(lobbyPanel);
            
        }
    }

    private void ClearLobbyPanels()
    {
        foreach (var lobbyPanel in _lobbyPanels)
        {
            Destroy(lobbyPanel.gameObject);
        }
        _lobbyPanels = new List<LobbyPanel>();
    }

}
