using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public bool isAscending;
    public List<Transform> slots = new List<Transform>();
    public Dictionary<Transform, bool> _slotsDictionary = new Dictionary<Transform, bool>();

    public NetworkVariableInt turnCounter = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    private NetworkVariableBool _gameStarted = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public static Stack<Card> currentCardsPlay = new Stack<Card>();

    private List<NetworkClient> _clients = new List<NetworkClient>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        foreach (var s in slots)
        {
            _slotsDictionary.Add(s, false);
        }

        turnCounter.Value = 1;
        Screen.SetResolution(1920, 1080, false);
    }

    private void OnEnable()
    {
        turnCounter.OnValueChanged += OnTurnCounterChanged;
        _gameStarted.OnValueChanged += OnGameStarted;
    }

    private void OnDisable()
    {
        turnCounter.OnValueChanged -= OnTurnCounterChanged;
        _gameStarted.OnValueChanged -= OnGameStarted;
    }

    private void OnTurnCounterChanged(int oldValue, int newValue)
    {
        turnCounter.Value = newValue;
    }

    private void OnGameStarted(bool oldValue, bool newValue)
    {
        _gameStarted.Value = newValue;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            PlacePosition();
            // if (_gameStarted.Value)
            // {
            //     EndTurn();
            // }
        }

        GUILayout.EndArea();
    }

    private static void StartButtons()
    {
        if (GUILayout.Button("Host", GUILayout.Width(150f), GUILayout.Height(50f)))
            NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client", GUILayout.Width(150f), GUILayout.Height(50f)))
            NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server", GUILayout.Width(150f), GUILayout.Height(50f)))
            NetworkManager.Singleton.StartServer();
    }

    private void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        if (_gameStarted.Value)
        {
            // GUILayout.Label("Player turn : " +
            //                 (turnCounter.Value % _clients.Count == 0 ? 1 : turnCounter.Value % _clients.Count));
            GUILayout.Label("Turn counter : " + turnCounter.Value);
        }
    }

    // private void EndTurn()
    // {
    //     if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
    //         out var networkedClient))
    //     {
    //         if (networkedClient.PlayerObject.GetComponent<Player>().canPlay.Value)
    //         {
    //             if (GUILayout.Button("End turn"))
    //             {
    //                 _clients[(turnCounter.Value-1)%_clients.Count].PlayerObject.GetComponent<Player>().canPlay.Value = false;
    //                 turnCounter.Value++;
    //                 _clients[(turnCounter.Value-1)%_clients.Count].PlayerObject.GetComponent<Player>().canPlay.Value = true;
    //                 networkedClient.PlayerObject.GetComponent<Player>().EndTurnServerRpc(false);
    //                 _clients[(turnCounter.Value - 1) % _clients.Count].PlayerObject.GetComponent<Player>()
    //                     .EndTurnServerRpc(true);
    //             }  
    //         }
    //     }
    // }

    // [ClientRpc]
    // public void UpdateTurnClientRpc()
    // {
    //     turnCounter.Value++;
    //     _clients[(turnCounter.Value-1)%_clients.Count].PlayerObject.GetComponent<Player>().canPlay.Value = true;
    // }
    
    private void PlacePosition()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<Player>();
            if (player && !player.isPlaced)
            {
                player.PlacePlayers();
                player.isPlaced = true;
            }
        }

        if (NetworkManager.Singleton.IsServer && !_gameStarted.Value)
        {
            if (GUILayout.Button("Start Game"))
            {
                CardDistribution();
            }
        }
    }

    private void CardDistribution()
    {
        _clients = NetworkManager.Singleton.ConnectedClientsList;
        _clients[(turnCounter.Value-1)%_clients.Count].PlayerObject.GetComponent<Player>().canPlay.Value = true;
        _gameStarted.Value = true;
        foreach (var client in _clients)
        {
            client.PlayerObject.GetComponent<Player>().RetrieveCardServerSideServerRpc();
        }
    }
}