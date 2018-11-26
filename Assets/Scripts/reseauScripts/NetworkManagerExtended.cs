using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerExtended : NetworkManager
{
    public UnetGameRoom GameRoom;
//    public GameObject playerPrefab;
    void Awake()
    {
        if (GameRoom == null)
        {
            Debug.LogError("Game Room property is not set on NetworkManager");
            return;
        }

        // Subscribe to events
        GameRoom.PlayerJoined += OnPlayerJoined;
        GameRoom.PlayerLeft += OnPlayerLeft;
    }

    /// <summary>
    /// Regular Unet method, which gets called when client disconnects from game server
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        // Don't forget to notify the room that a player disconnected
        GameRoom.ClientDisconnected(conn);
    }

    /// <summary>
    /// Invoked, when client provides a successful access token and enters the room
    /// </summary>
    /// <param name="player"></param>
    private void OnPlayerJoined(UnetMsfPlayer player)
    {
        Debug.Log("Player joined");
        // Create an instance of the player prefab we made earlier
        var playerObj = Instantiate(playerPrefab);

        //find all spawn points, we just have the one, but you can add more if you like

        //add the connection to the game server to make sure the player can update on the client and the gameserver
        NetworkServer.AddPlayerForConnection(player.Connection, playerObj.gameObject, 0);

        return;
    }


    private void OnPlayerLeft(UnetMsfPlayer player)
    {

    }

    public void DisconnectAllPlayers()
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            player.connectionToClient.Disconnect();
        }
    }
}