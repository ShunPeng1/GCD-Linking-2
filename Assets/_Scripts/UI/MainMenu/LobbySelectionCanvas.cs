using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


public class LobbySelectionCanvas : MonoBehaviour
{
    
    [SerializeField] private Button _createLobby;
    [SerializeField] private Button _quickJoinLobby;
    [SerializeField] private Button _refreshLobby;

    [Header("Lobby List")] 
    [SerializeField] private RectTransform _lobbyContent;
    [SerializeField] private AvailableLobbyPanel _availableLobbyPanelPrefab;
    private List<AvailableLobbyPanel> _lobbyPanels = new List<AvailableLobbyPanel>();

    // Start is called before the first frame update
    void Start()
    {
        _createLobby.onClick.AddListener( CreateLobby);
        _quickJoinLobby.onClick.AddListener( QuickJoinLobby);
        _refreshLobby.onClick.AddListener(RefreshLobby);
    }

    private async void CreateLobby()
    {
        await MultiplayerManager.Instance.CreateLobby();
        AssetSceneManager.Instance.LoadNextScene();
    }

    private async void QuickJoinLobby()
    {
        await MultiplayerManager.Instance.QuickJoinLobby();
        AssetSceneManager.Instance.LoadNextScene();
    }

    private async void RefreshLobby()
    {
        QueryResponse queryResponse = await MultiplayerManager.Instance.QueryLobbiesAsync();
        
        ClearLobbyPanels();
        
        foreach (var lobby in queryResponse.Results)
        {
        
            var lobbyPanel = Instantiate(_availableLobbyPanelPrefab, _lobbyContent);
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
        _lobbyPanels = new List<AvailableLobbyPanel>();
    }

}
