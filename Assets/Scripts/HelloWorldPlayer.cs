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

    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }

    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    public void Update()
    {
        transform.position = position.Value;
    }
}