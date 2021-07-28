using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public List<Card> selectedCards = new List<Card>();
    public void SortCard()
    {
        int i = 0;
        foreach (Transform card in transform)
        {
            var position = card.localPosition;
            position = new Vector3(position.x+i*.7f, position.y, position.z - (i / 1000f));
            card.localPosition = position;
            card.GetComponentInChildren<Card>().position.Value = Vector3.zero;
            i++;
        }
    }
}