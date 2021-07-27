using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;

public class HelloWorldManager : MonoBehaviour
{
    public static HelloWorldManager instance;
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

            // SubmitNewPosition();
            PlacePosition();
        }

        GUILayout.EndArea();
    }

    private static void StartButtons()
    {
        if (GUILayout.Button("Host", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartServer();
    }

    private static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    public void PlacePosition()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<HelloWorldPlayer>();
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
    
    public void CardDistribution()
    {
        var deck = DeckOfCard.instance;
        deck.Shuffle();

        var clients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < 7 * clients.Count; i++)
        {
            var card = Instantiate(cardPrefab,
                    clients[i % clients.Count].PlayerObject.GetComponent<Player>().hand.transform)
                .GetComponentInChildren<Card>();
            card.card = deck.cards[i];
            card.InitCard(clients[i % clients.Count].PlayerObject.GetComponent<Player>().hand);
            clients[i % clients.Count].PlayerObject.GetComponent<Player>().hand.cards.Add(card);
        }

        for (int i = 0; i < clients.Count; i++)
        {
            clients[i].PlayerObject.GetComponent<Player>().hand.SortCard();
        }
    }

    public void PlayCard(Hand h)
    {
        currentHandPlay.cards = currentHandPlay.cards.Concat(h.selectedCards).ToList();
    }
}