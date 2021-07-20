using System;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;
  public bool isAscending;
  public Card cardPrefab;

  private void Awake()
  {
    instance = this;
  }

  public void CardDistribution()
  {
    var deck = DeckOfCard.instance;
    deck.Shuffle();

    var clients = NetworkManager.Singleton.ConnectedClientsList;

    for (int i = 0; i < deck.cards.Count; i++)
    {
      var card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
      card.card = deck.cards[i];
      clients[i%clients.Count].PlayerObject.GetComponent<Player>().hand.cards.Add(card);
    }
  }
}