using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFSW.QC;
using TurnTheGameOn.Timer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityUtilities;
using Random = UnityEngine.Random;

[Serializable]
public enum EncryptionType
{
    Dtls, // Datagram Transport Layer Security
    Wss, // Web Socket Secure for Web GL
    Upd,
    Ws
}


public class MultiplayerManager : PersistentSingletonMonoBehaviour<MultiplayerManager>
{
    
    public string PlayerName { get; set; }
    public string PlayerId { get; set; }
    
    [Header("Lobby")]
    [SerializeField] private string _lobbyName = "Lobby Name";
    [SerializeField] private int _maxPlayers = 4;
    [SerializeField] private EncryptionType _encryptionType = EncryptionType.Dtls;

    [Header("Timer")]
    [SerializeField] private Timer _heartBeatTimer; // The lobby will timeout if there is no message sending in 30 seconds
    [SerializeField] private Timer _pollForUpdateTimer;
    
    private Lobby _currentLobby;

    private const string K_KEY_JOIN_CODE = "RelayJoinCode";

    private const string DTLS_ENCRYPTION = "dtls";
    private const string UDP_ENCRYPTION = "udp";
    private const string WSS_ENCRYPTION = "wss";
    private string ConnectionType => _encryptionType == EncryptionType.Dtls ? DTLS_ENCRYPTION : WSS_ENCRYPTION;
    
    async void Start()
    {
        await Authenticate();
        
        SetUpTimer();
    }

    private async Task Authenticate()
    {
        await Authenticate("Player_" + Random.Range(0, 1024));
    }

    private async Task Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            // Create a profile for player
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(playerName);
            
            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    private void SetUpTimer()
    {

        _heartBeatTimer.timesUpEvent.AddListener(() => HandleHeartBeatAsync());

    }
    
    public async Task CreateLobby()
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                {
                    Profile = new PlayerProfile(AuthenticationService.Instance.Profile),
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName)}
                    }
                }
            };

            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);
            Debug.Log("Created lobby :" + _currentLobby.Name + " with code "+ _currentLobby.LobbyCode);
            
            // heartbeat timer and poll for updates
            _heartBeatTimer.StartTimer();
            _pollForUpdateTimer.StartTimer();

            await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                       
                    K_KEY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                     
                    }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(
                allocation, ConnectionType));

            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Fail to create lobby: "+ e.Message);
        }
    }

    public async Task QuickJoinLobby()
    {
        try
        {
            _currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            _pollForUpdateTimer.StartTimer();

            string relayJoinCode = _currentLobby.Data[K_KEY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(
                joinAllocation, ConnectionType));

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Fail to quick join lobby: "+ e.Message);
        }
    }

    async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers - 1); // excluding the host
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to allocate relay: "+ e.Message);
            return default;
        }
    }


    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to get relay join code: "+ e.Message);
            return default;
        }
    }

    async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: "+ e.Message);
            return default;
        }
    }
    
    async Task JoinLobbyByCode(string joinCode)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
            Debug.Log("Join lobby with code: "+ joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: "+ e.Message);
        }
    }
    
    private async Task HandleHeartBeatAsync()
    {
        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            Debug.Log("Heart beat Lobby ");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to heartbeat lobby: "+ e.Message);
            throw;
        }
    }

    private async Task GetLobbyAsync()
    {
        try
        {
            await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to heartbeat lobby: "+ e.Message);
            throw;
        }
    }
    
    
    private async Task QueryLobbiesAsync()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GE)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            Debug.Log($"lobbies found: {queryResponse.Results}");
            foreach (var lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to query lobbies: "+ e.Message);
            throw;
        }
    }
 }
