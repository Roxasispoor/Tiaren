using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TransmitterNoThread : MonoBehaviour
{

    public TcpClient client;
    public TcpListener tcpListener;
    public TcpClient server;
    public NetworkManager networkManager;
    private string betweenFilesChar = "///";
    private string endChar = "bite";
    private string stringGrid = "Grid";
    private string stringObjectOnBloc = "ObjectOnBlocs";
    private string stringLiving = "Living";
    private string pathNewGrid = "NewGrid.json";
    private string pathNewLiving = "Living.json";

    // Use this for initialization
    private void Start()
    {

    }
    /// <summary>
    /// Listen to tcp request. When clients connects, imediately close listenning socket, connect to client and send him files. Non-blocking  
    /// </summary>
    /// <returns></returns>
    public IEnumerator AcceptTcp()
    {

        string adress = networkManager.networkAddress;
        if (adress == "localhost")
        {
            adress = "127.0.0.1";
        }
        if (tcpListener != null)
        {
            tcpListener.Stop();
            tcpListener = null;
            Debug.LogError("Destroyed tcplistenner for new reconnection");
        }
        tcpListener = new TcpListener(IPAddress.Parse(adress), networkManager.matchPort);
        tcpListener.Start();
        while (client == null)
        {
            //Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also user server.AcceptSocket() here.
            if (tcpListener.Pending())
            {
                Debug.LogError("Client detected");
                client = tcpListener.AcceptTcpClient();
                SendData();
            }
            else
            {
                yield return null;
            }
        }
    }
    /// <summary>
    /// Save the current grid and send it with predefined files to client
    /// </summary>
    public void SendData()
    {
        if (client == null)
        {
            Debug.LogError("Client was null when sending data");
        }
        else
        {
            //WARNING: Do not have the file open in notepad or whatsoever
            Grid.instance.SaveGridNetwork();

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {


                    SendFile(pathNewGrid, stream, stringGrid);
                    LivingPlaceable[] livings = FindObjectsOfType<LivingPlaceable>();
                    foreach (LivingPlaceable living in livings)
                    {
                        living.Save();
                        SendFile(pathNewLiving, stream, stringLiving);

                    }
                    ObjectOnBloc[] objsToSend = FindObjectsOfType<ObjectOnBloc>();
                    string textToSave = "";
                    foreach (ObjectOnBloc obj in objsToSend)
                    {
                        textToSave += obj.Save() + "\n";
                    }
                    textToSave += stringObjectOnBloc;
                    File.WriteAllText(stringObjectOnBloc + ".json", textToSave);
                    SendFile(stringObjectOnBloc + ".json", stream, stringObjectOnBloc);

                    byte[] endOfFileByte = Encoding.ASCII.GetBytes(endChar);
                    // Write byte array to socketConnection stream.               
                    stream.Write(endOfFileByte, 0, endOfFileByte.Length);


                    Debug.LogError("Server sent his message - should be received by client");
                    stream.Close();
                    client.Close();
                    client = null;
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }
    /// <summary>
    /// Sends the file specified in path, from build location.
    /// Format will be File => type => /// (in ascii)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="type"></param>
    private void SendFile(string path, NetworkStream stream, string type)
    {
        StreamReader reader = new StreamReader(path);

        string serverMessage = reader.ReadToEnd();
        // Convert string message to byte array.                 
        byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
        byte[] betweenFilesByte = Encoding.ASCII.GetBytes(betweenFilesChar);
        byte[] typeByte = Encoding.ASCII.GetBytes(type);
        // Write byte array to socketConnection stream.               
        stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
        stream.Write(typeByte, 0, typeByte.Length);
        stream.Write(betweenFilesByte, 0, betweenFilesByte.Length);
        reader.Close();

    }

    /// <summary>
    /// Connect to server and listen for the data incoming until it receive EOF
    /// player is localClient that reconnects
    /// WARNING: prefabList must be kept up to date and contain all living with index being their serialize id
    /// </summary>
    /// <returns></returns>
    public IEnumerator ListenToData(Player player)
    {
        server = new TcpClient("localhost", networkManager.matchPort);
        Byte[] bytes = new Byte[1024];
        string serverMessage = "";
        string totalMessage = "";
        using (NetworkStream stream = server.GetStream())
        {
            while (!serverMessage.EndsWith(endChar))
            {

                try
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while (stream.DataAvailable && (length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        byte[] incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        serverMessage = Encoding.ASCII.GetString(incommingData);
                        totalMessage += serverMessage;
                        Debug.Log("server message received as: " + serverMessage);
                    }

                }
                catch (SocketException socketException)
                {
                    Debug.Log("Socket exception: " + socketException);
                }
                yield return null;
            }

            Debug.Log("Coroutine, splitting now");
            string[] files = totalMessage.Split(betweenFilesChar.ToCharArray());
            Debug.Log(files);
            GameManager.instance.player2.GetComponent<Player>().Characters.Clear();
            GameManager.instance.player1.GetComponent<Player>().Characters.Clear();
            //Parse type to see what to create
            foreach (string str in files)
            {


                if (str.EndsWith(stringGrid))
                {
                    Debug.Log("File grid detected");
                    string strCopy = str.TrimEnd(stringGrid.ToCharArray());

                    GameManager.instance.ResetGrid();
                    //Deletes old grid, fill it anew

                    Grid.instance.FillGridAndSpawnNetwork(GameManager.instance.gridFolder, strCopy);
                }

                else if (str.EndsWith(stringLiving))
                {
                    Debug.Log("Living detected");
                    string strCopy = str.TrimEnd(stringLiving.ToCharArray());

                    string line = "";
                    StreamReader reader = new StreamReader(LivingPlaceable.GenerateStreamFromString(strCopy));
                    if ((line = reader.ReadLine()) == null)
                    {
                        Debug.Log("Empty file while reading living form file!");
                        yield return null;
                    }
                    Stats newLivingStats = JsonUtility.FromJson<Stats>(line);

                    //Spawns
                    if (newLivingStats.playerPosesser == "player1")
                    {
                        Debug.LogError("Create perso 1" + newLivingStats.serializeNumber);
                        GameObject charac = Instantiate(Grid.instance.prefabsList[newLivingStats.serializeNumber - 1],
                           newLivingStats.positionSave, Quaternion.identity);
                        LivingPlaceable charac1 = charac.GetComponent<LivingPlaceable>();

                        GameManager.instance.player1.GetComponent<Player>().Characters.Add(charac);
                        charac1.Player = GameManager.instance.player1.GetComponent<Player>();
                        Vector3Int posPers = new Vector3Int((int)newLivingStats.positionSave.x, (int)newLivingStats.positionSave.y,
                                (int)newLivingStats.positionSave.z);
                        Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = charac1;
                        charac1.Weapons.Add(Instantiate(GameManager.instance.prefabWeapons[0], charac.transform)); // to change in function of the start weapon
                        charac1.EquipedWeapon = charac1.Weapons[0].GetComponent<Weapon>();
                        charac1.InitNoClass();
                        charac1.LoadFromString(strCopy);
                        GameManager.instance.idPlaceable[charac1.netId] = charac1;
                        NetIdeable.currentMaxId = charac1.netId >= NetIdeable.currentMaxId ? charac1.netId + 1 : NetIdeable.currentMaxId;
                        foreach (Skill skill in charac1.Skills)
                        {

                            skill.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + skill.SkillName);
                        }
                    }
                    else if (newLivingStats.playerPosesser == "player2")
                    {                    
                        Debug.LogError("Create perso 2" + newLivingStats.serializeNumber);
                        GameObject charac = Instantiate(Grid.instance.prefabsList[newLivingStats.serializeNumber - 1],
                           newLivingStats.positionSave, Quaternion.identity);
                        LivingPlaceable charac1 = charac.GetComponent<LivingPlaceable>();

                        GameManager.instance.player2.GetComponent<Player>().Characters.Add(charac);
                        charac1.Player = GameManager.instance.player2.GetComponent<Player>();
                        Vector3Int posPers = new Vector3Int((int)newLivingStats.positionSave.x, (int)newLivingStats.positionSave.y,
                                (int)newLivingStats.positionSave.z);
                        Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = charac1;
                        charac1.Weapons.Add(Instantiate(GameManager.instance.prefabWeapons[0], charac.transform)); // to change in function of the start weapon
                        charac1.EquipedWeapon = charac1.Weapons[0].GetComponent<Weapon>();
                        charac1.InitNoClass();
                        charac1.LoadFromString(strCopy);

                        GameManager.instance.idPlaceable[charac1.netId] = charac1;
                        NetIdeable.currentMaxId = charac1.netId >= NetIdeable.currentMaxId ? charac1.netId + 1 : NetIdeable.currentMaxId;

                        foreach (Skill skill in charac1.Skills)
                        {

                            skill.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + skill.SkillName);
                        }


                    }

                }
                else if(str.EndsWith(stringObjectOnBloc))
                {
                    string strCopy = str.TrimEnd(stringLiving.ToCharArray());
                    string line = "";
                    StreamReader reader = new StreamReader(LivingPlaceable.GenerateStreamFromString(strCopy));
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] objectInfo = line.Split(';');
                        Vector3Int positionObj = StringToVector3Int(objectInfo[2]);
                        GameObject objectonbloc = Instantiate(Grid.instance.prefabsList[Int32.Parse(objectInfo[0])],
                       positionObj, Quaternion.identity, Grid.instance.GetPlaceableFromVector(positionObj).transform);
                        ObjectOnBloc objectOnBloc1 = objectonbloc.GetComponent<ObjectOnBloc>();
                        objectOnBloc1.netId = Int32.Parse(objectInfo[1]);
                        NetIdeable.currentMaxId = objectOnBloc1.netId >= NetIdeable.currentMaxId ? objectOnBloc1.netId + 1 : NetIdeable.currentMaxId;
                        objectOnBloc1.Load(objectInfo);

                    }
                  
                }
            }
            GameManager.instance.ResetAllBatches();
            Debug.Log("I ended the coroutine reconnect me now");
            player.CmdReconnectMe();
            //We suppose we found back our initial place thanks to start

            /* Player[] players = FindObjectsOfType<Player>();
             foreach(Player player in players)
             {
                 if(player.isLocalPlayer)//Contact everyone to tell them
                 {
                     player.CmdReconnectMe();
                 }
             }*/
        }
        server.Close();
        server=null;
    }
    public static Vector3Int StringToVector3Int(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3Int result = new Vector3Int(
            Int32.Parse(sArray[0]),
            Int32.Parse(sArray[1]),
            Int32.Parse(sArray[2]));

        return result;
    }
}
