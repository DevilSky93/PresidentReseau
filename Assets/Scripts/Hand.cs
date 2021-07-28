using System.Collections.Generic;
using DG.Tweening;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Hand : NetworkBehaviour
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

    [ServerRpc]
    public void PlayCardServerRpc()
    {
        Debug.Log("Play !");
        PlayCardClientRpc();
    }

    [ClientRpc]
    private void PlayCardClientRpc()
    {
        if (selectedCards.Count <= 0) return;
        if (GameManager.currentCardsPlay.Count > 0)
        {
            if (selectedCards[0].value.Value >= GameManager.currentCardsPlay.Peek().value.Value)
            {
                MoveCards();
            }
            else
            {
                foreach (var sc in selectedCards)
                {
                    sc.CardCancelled();
                    transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.OutCubic);
                }
            }
        }
        else
        {
            MoveCards();
        }
        
        selectedCards.Clear();   
    }

    [ServerRpc]
    public void EndTurnServerRpc()
    {
        
    }

    [ClientRpc]
    public void EndTurnClientRpc()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            out var networkedClient))
        {
            networkedClient.PlayerObject.GetComponent<Player>().canPlay.Value = false;
        }
    }

    private void MoveCards()
    {
        foreach (var card in selectedCards)
        {
            card.GetComponent<Rigidbody>().isKinematic = false;
            card.transform.DOMove(new Vector3(0, 3f, 0), .2f).SetEase(Ease.OutCubic).OnStart(() =>
            {
                cards.Remove(card);
                card.CardPlay();
                GameManager.currentCardsPlay.Push(card);
            }).OnUpdate(() =>
            {
                card.position.Value = transform.localPosition;
            }).OnComplete(() =>
            {
                card.transform.parent.SetParent(null);
            });
        }
    }
}