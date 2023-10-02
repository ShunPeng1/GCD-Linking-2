using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private MenuCanvas _menuCanvas;
    [SerializeField] private LobbySelectionCanvas _lobbySelectionCanvas;
    
    [SerializeField] private Button _guestLoginButton;
    [SerializeField] private Button _googleLoginButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _guestLoginButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.AuthenticateAnonymously();
            DisplayLobbySelection();
        });
        
    }
    
    
    private void DisplayLobbySelection()
    {
        _menuCanvas.gameObject.SetActive(false);
        _lobbySelectionCanvas.gameObject.SetActive(true);
    }
    
}
