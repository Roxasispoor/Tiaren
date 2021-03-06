﻿using Barebones.MasterServer;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerExtended : NetworkManager
{
    public UnetGameRoom GameRoom;
    [NonSerialized]
    public bool launchedFromMaster = false;
    //    public GameObject playerPrefab;
    void Awake()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        bool launchedFromMenu = false;
        foreach (string arg in arguments)
        {
          //  Debug.LogError(arg +"\n");
            if(arg=="launchedFromMenu")
            {
                launchedFromMenu = true;
            }
        }
        if (Msf.Args.AssignedPort != -1)
        {
            networkPort = Msf.Args.AssignedPort;
        }
        Debug.LogError("mynetworkPort" + networkPort);
        if (launchedFromMenu)
        {
            foreach (string arg in arguments)
            {
                if (arg == "server")
                {

                    StartServer();
                      
                }
                if (arg == "client")
                {
                    StartClient();
                }
            }
        }


        if (GameRoom == null)
        {
            Debug.LogError("Game Room property is not set on NetworkManager");
            
        }
        else
        { 
        // Subscribe to events
        GameRoom.PlayerJoined += OnPlayerJoined;
        GameRoom.PlayerLeft += OnPlayerLeft;
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log(launchedFromMaster);
        if(!launchedFromMaster)
        {

            Debug.LogError("onclientConnectCalled");
            ClientScene.AddPlayer(conn, 0);

        }
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.LogError("onserveraddplayercalled");
        var player = Instantiate(playerPrefab);
       // player.GetComponent<Player>().username = username; //Not very secured, session could be highjacked, should be instead a token given when game is launched
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
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
        Debug.LogError("Player trigerred OnPlayerJoined");

        // Create an instance of the player prefab we made earlier
        var playerObj = Instantiate(playerPrefab);

        
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