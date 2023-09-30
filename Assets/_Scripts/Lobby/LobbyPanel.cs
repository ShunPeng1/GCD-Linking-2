using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    private Lobby _lobby;
    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private TMP_Text _lobbyName;

    public void Build(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyName.text = _lobby.Name;
        
        _joinLobbyButton.onClick.AddListener(JoinLobby);
    }

    private async void JoinLobby()
    {
        await MultiplayerManager.Instance.JoinLobbyById(_lobby.Id);
    } 
    
}
