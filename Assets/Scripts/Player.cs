using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Spawning;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariableVector3 position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableVector3 rotation = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableBool canPlay = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    // public NetworkVariable<List<Card>> hand = new NetworkVariable<List<Card>>(new NetworkVariableSettings
    // {
    //   WritePermission  = NetworkVariablePermission.ServerOnly,
    //   ReadPermission = NetworkVariablePermission.Everyone
    // });
    public Hand hand;
    public GameObject cardPrefab;

    public bool isPlaced;

    public Camera cam;

    private Transform _currentSlot;

    public void PlacePlayers()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var pos = PositionSlotsPlayer();
            transform.position = pos.position;
            transform.eulerAngles = pos.eulerAngles;
            position.Value = pos.position;
            rotation.Value = pos.eulerAngles;
        }
        else
        {
            PlacePlayerServerRpc();
        }
    }

    [ServerRpc]
    private void PlacePlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = PositionSlotsPlayer().position;
        rotation.Value = PositionSlotsPlayer().eulerAngles;
    }

    private Transform PositionSlotsPlayer()
    {
        foreach (var slot in GameManager.instance.slots.Where(s =>
            !GameManager.instance._slotsDictionary[s] && _currentSlot == null))
        {
            GameManager.instance._slotsDictionary[slot] = true;
            _currentSlot = slot;
            return _currentSlot;
        }

        return _currentSlot;
    }

    public void Update()
    {
        transform.position = position.Value;
        transform.eulerAngles = rotation.Value;

        if (!IsLocalPlayer)
        {
            cam.enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RetrieveCardServerSideServerRpc()
    {
        RetrieveListClientRpc();
    }

    [ClientRpc]
    private void RetrieveListClientRpc(ClientRpcParams clientRpcParams = default)
    {
        for (int i = 0; i < 7; i++)
        {
            var c = InstantiateCard(i);
            c.transform.root.SetParent(hand.transform);
        }
        hand.SortCard();
    }

    private GameObject InstantiateCard(int idx)
    {
        var deck = DeckOfCard.instance;
        deck.Shuffle();

        GameObject c = Instantiate(cardPrefab, hand.transform);
        // c.GetComponent<NetworkObject>().Spawn();
        c.GetComponentInChildren<Card>().card = deck.cards[idx];
        deck.cards.RemoveAt(idx);
        c.GetComponentInChildren<Card>().InitCard(hand);
        hand.cards.Add(c.GetComponentInChildren<Card>());

        return c;
    }
}