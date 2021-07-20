using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public bool isAscending;

  public void CardDistribution()
  {
    var deck = DeckOfCard.instance;
    deck.Shuffle();
    
  }
}