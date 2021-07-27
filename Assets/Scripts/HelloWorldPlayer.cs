using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class HelloWorldPlayer : NetworkBehaviour
{
    public NetworkVariableVector3 position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public bool isPlaced;
    
    public override void NetworkStart()
    {
        Move();
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    public void PlacePlayers()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var pos = PositionSlotsPlayer();
            transform.position = pos.position;
            transform.eulerAngles = pos.eulerAngles;
            position.Value = pos.position;
        }
        else
        {
            PlacePlayerServerRpc();
        }
    }

    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }
    
    [ServerRpc]
    private void PlacePlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = PositionSlotsPlayer().position;
    }

    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    private static Transform PositionSlotsPlayer()
    {
        for (int i = 0; i < GameManager.instance.slots.Count; i++)
        {
            if (!GameManager.instance._slotsDictionary[GameManager.instance.slots[i]])
            {
                GameManager.instance._slotsDictionary[GameManager.instance.slots[i]] = true;
                return GameManager.instance.slots[i];
            }
            
        }
        return null;
    }

    public void Update()
    {
        transform.position = position.Value;
    }
}