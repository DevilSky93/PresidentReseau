using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using UnityEngine;
using Random = System.Random;

public class DeckOfCard : NetworkBehaviour
{
  public static DeckOfCard instance;
  public List<CardData> cards = new List<CardData>(52);

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    Shuffle();
  }

  public void Shuffle()
  {
    Random rnd = new Random();
    CardData[] shuffleDeck = cards.OrderBy(x => rnd.Next()).ToArray();

    cards = shuffleDeck.ToList();
  }
}