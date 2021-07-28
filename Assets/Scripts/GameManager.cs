using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Spawning;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isAscending;
    public GameObject cardPrefab;
    public List<Transform> slots = new List<Transform>();
    public Dictionary<Transform, bool> _slotsDictionary = new Dictionary<Transform, bool>();

    private Hand currentHandPlay;

    private List<NetworkClient> _clients = new List<NetworkClient>();
    private void Awake()
    {
        instance = this;
        foreach (var s in slots)
        {
            _slotsDictionary.Add(s, false);
        }

        Screen.SetResolution(1920, 1080, false);
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

    private static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

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

        if (NetworkManager.Singleton.IsServer)
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
        
        foreach (var client in _clients)
        {
            client.PlayerObject.GetComponent<Player>().RetrieveCardServerSideServerRpc();
        }
    }

    public void PlayCard(Hand h)
    {
        currentHandPlay.cards = currentHandPlay.cards.Concat(h.selectedCards).ToList();
    }
}