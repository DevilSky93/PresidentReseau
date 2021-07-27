using MLAPI;
using UnityEngine;

public class HelloWorldManager : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            // SubmitNewPosition();
            PlacePosition();
        }

        GUILayout.EndArea();
    }

    private static void StartButtons()
    {
        if (GUILayout.Button("Host", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server", GUILayout.Width(150f), GUILayout.Height(50f))) NetworkManager.Singleton.StartServer();
    }

    private static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    public static void PlacePosition()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<HelloWorldPlayer>();
                if (player && !player.isPlaced)
                {
                    player.PlacePlayers();
                    player.isPlaced = true;
                }
            }

        if (NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start Game"))
            {
                GameManager.instance.CardDistribution();
            }   
        }
    }
}