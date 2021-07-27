using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class GameManager : NetworkBehaviour
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
    }

    public void CardDistribution()
    {
        var deck = DeckOfCard.instance;
        deck.Shuffle();

        var clients = NetworkManager.Singleton.ConnectedClientsList;

        // for (int i = 0; i < clients.Count; i++)
        // {
        //   var hand = Instantiate(UIManager.instance.playerHand, UIManager.instance.transform);
        //   clients[i].PlayerObject.GetComponent<Player>().hand = hand;
        // }

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