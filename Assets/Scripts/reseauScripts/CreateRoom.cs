using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Barebones.MasterServer
{

    /// <summary>
    ///     Game creation window
    /// </summary>
    public class CreateRoom : MonoBehaviour
    {
        RoomAccessPacket access;
        //Allows the map name and scene to be set in the Inspector as a single object.
        //This is useful if you want to have multiple scenes available to choose from,
        //although, we will only have the one. 
        [Serializable]
        public class MapSelection
        {
            public string Name;
            public SceneField Scene;
        }

        public MapSelection map;

        //this will hold the reference to the request for the game instance to spawn
        SpawnRequestController Request;
        private Coroutine WaitConnectionCoroutine;

        //This will be called when we click the button that this script will be attached to
        public void OnCreateClick()
        {
            //LOG IN AS GUEST
            //This is part of the error in judgment. In order for the serverside code to work
            //and the gameServer to connect to the client properly, it needs to be authenticated. 
            //To get around this, we're going to trigger the "Guest access" by default before we 
            //spawn the gameserver.
            //I will provide a tutorial at a later date to explain how the authModule works and how
            //to write your own.
            var promise = Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Logging in");
            Msf.Client.Auth.LogInAsGuest((accInfo, error) =>
            {
                promise.Finish();

                if (accInfo == null)
                {
                    Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(error));
                    Logs.Error(error);
                }
            });


            //There can be more or fewer settings included, we're going to keep this simple.
            var settings = new Dictionary<string, string>
            {
                {MsfDictKeys.MaxPlayers, "20"},
                {MsfDictKeys.RoomName, map.Name},
                {MsfDictKeys.MapName, map.Name},
                {MsfDictKeys.SceneName, map.Scene.SceneName}
            };

            //The actual request message sent to the Spawner.
            //This returns a callback containing the SpawnRequestController that will allow us to track 
            //the progress of the spawn request
            Msf.Client.Spawners.RequestSpawn(settings, "", (requestController, errorMsg) =>
            {
                //If something went wrong, the request will return "null" and an error will be included
                if (requestController == null)
                {
                    Msf.Events.Fire(Msf.EventNames.ShowDialogBox,
                        DialogBoxData.CreateError("Failed to create a game: " + errorMsg));

                    Logs.Error("Failed to create a game: " + errorMsg);
                }

                //If the Controller is not null, we can track its progress
                TrackRequest(requestController);
            });
        }

        //Set up the Request variable to run OnStatusChange() when the status of our spawn request is changed
        public void TrackRequest(SpawnRequestController request)
        {
            if (Request != null)
                Request.StatusChanged -= OnStatusChange;

            if (request == null)
                return;

            request.StatusChanged += OnStatusChange;

            Request = request;
            gameObject.SetActive(true);

            //Typically we'd include an "abort" option, but in the spirit of keeping things simple,
            //we're leaving it out for this tutorial
        }

        protected void OnStatusChange(SpawnStatus status)
        {
            //We'll display the current status of the request
            Debug.Log(string.Format("Progress: {0}/{1} ({2})", (int)Request.Status, (int)SpawnStatus.Finalized, Request.Status));

            //If game was aborted
            if (status < SpawnStatus.None)
            {
                Debug.Log("Game creation aborted");
                return;
            }

            //Once the SpawnStatus reaches the Finalized state we can get the data from the finished request
            if (status == SpawnStatus.Finalized)
            {
                Request.GetFinalizationData((data, error) =>
                {
                    if (data == null)
                    {
                        Msf.Events.Fire(Msf.EventNames.ShowDialogBox,
                            DialogBoxData.CreateInfo("Failed to retrieve completion data: " + error));

                        Logs.Error("Failed to retrieve completion data: " + error);

                        Request.Abort();
                        return;
                    }

                    // Completion data received
                    OnFinalizationDataRetrieved(data);
                });
            }
        }
       
        
      
    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
            var networkManager = FindObjectOfType<NetworkManagerExtended>();
            networkManager.networkAddress = access.RoomIp;
            networkManager.networkPort = access.RoomPort;

            // Start connecting
            networkManager.StartClient();

            // do whatever you like
        }
        public void OnFinalizationDataRetrieved(Dictionary<string, string> data)
        {
            Debug.Log("Start on finalization retrieved");
            //This comes from the "CreateGameProgressUi.cs" script. I'm not sure what could cause 
            //this error at this time, but it feels safer to leave it.
            if (!data.ContainsKey(MsfDictKeys.RoomId))
            {
                throw new Exception("Game server finalized, but didn't include room id");
            }

            var roomId = int.Parse(data[MsfDictKeys.RoomId]);

            //The request has finished, the game instance is made. Now we need to actually get access to the room.
            Msf.Client.Rooms.GetAccess(roomId, (access, error) =>
            {
                if (access == null)
                {
                    Msf.Events.Fire(Msf.EventNames.ShowDialogBox,
                            DialogBoxData.CreateInfo("Failed to get access to room: " + error));

                    Logs.Error("Failed to get access to room: " + error);

                    return;
                }
                SceneManager.LoadScene("online");
                this.access = access;
                SceneManager.sceneLoaded += OnSceneLoaded;
                //StartCoroutine(WaitSceneLoaded(access));
            }
            );
           

            // Quick runthrough of what "GetAccess" does, as I found it complicated to trace:
            // The method is found in "MsfRoomsClient.cs". After checking that the Client is connected to the server,
            //     the request is forwarded to "RoomsModule.cs".
            // The RoomsModule checks to make sure that the requested room exists, then sends a request to the room for access.
            // This request is handled by "RegisteredRoom.cs". This script checks to make sure that this is not a duplicate request,
            //     that there isn't an unclaimed request token, and that the room isn't full.
            // If all the checks are passed, a message is sent that is handled by "RoomController.cs". This method obtains the 
            //     RoomController in the game instance. The accessProvider in that Controller is invoked and returns the RoomAccessPacket.
            // The RoomController responds to the RegisteredRoom which invokes the callback to the RoomsModule which responds to the 
            //     MsfRoomsClient. (Basically, everything falls back to the start)
            // The MsfRoomsClient then invokes the RoomConnector with the access packet. In this example, the connector instance is 
            //     handled by UnetRoomConnector.cs. This script changes to the proper scene if needed, then waits for the connection.
            // At this point, Logger.Info("Connected to game server, about to send access"); will be called. You can see this in the 
            //     console if the logging level is Info or lower.
            // The request is then sent to UnetGameRoom.cs, which validates the access request on the serverside, gets the account
            //     information, and finally calls "OnPlayerJoined()" which invokes "PlayerJoined", which will result in a call back 
            //     in our "ScratchNetworkManager" script. (This would normally end up in "NetworkManagerSample.cs" for the "quicksetup" demo

            // That's the quick version. For a quicker version (tl:dr):

            //     The access request is sent out, validated by several scripts on the client side, the client is switched to the
            //     proper scene, the access request is sent to the server, re-validated, then the final call for the player joining is 
            //     called in the game instance.
            // 
        }

        private void ShowError(string error)
        {
            Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(error));
        }
    }
}