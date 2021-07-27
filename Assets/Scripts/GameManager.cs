using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
  public static GameManager instance;
  public bool isAscending;
  public GameObject cardPrefab;

  private Hand currentHandPlay;

  private List<NetworkClient> _clients = new List<NetworkClient>();
  private void Awake()
  {
    instance = this;
  }

  public void CardDistribution()
  {
    var deck = DeckOfCard.instance;
    deck.Shuffle();

    var clients = NetworkManager.Singleton.ConnectedClientsList;

    for (int i = 0; i < clients.Count; i++)
    {
      var hand = Instantiate(UIManager.instance.playerHand, UIManager.instance.transform);
      clients[i].PlayerObject.GetComponent<Player>().hand = hand;
    }
    
    for (int i = 0; i < deck.cards.Count; i++)
    {
      var card = Instantiate(cardPrefab, clients[i%clients.Count].PlayerObject.GetComponent<Player>().hand.transform).GetComponentInChildren<Card>();
      card.card = deck.cards[i];
      card.InitCard(clients[i%clients.Count].PlayerObject.GetComponent<Player>().hand);
      clients[i%clients.Count].PlayerObject.GetComponent<Player>().hand.cards.Add(card);
      clients[i%clients.Count].PlayerObject.GetComponent<Player>().hand.SortCard();
    }

    // clients = NetworkManager.ConnectedClientsList;
    // Debug.Log(clients.Count);
  }

  public void PlayCard(Hand h)
  {
    currentHandPlay.cards = currentHandPlay.cards.Concat(h.selectedCards).ToList();
  }
}