using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TransmitterNoThread : MonoBehaviour {

    public TcpClient client;
    public TcpListener tcpListener;
    public TcpClient server;
    public NetworkManager networkManager;

    // Use this for initialization
    void Start () {
		
	}
    //TcpClient client
    public IEnumerator AcceptTcp()
    {

        string adress = networkManager.networkAddress;
        if (adress == "localhost")
        {
            adress = "127.0.0.1";
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
    public void SendData()
    {
        if (client == null)
        {
            Debug.LogError("Client was null when sending data");
        }
        else
        { 

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    string serverMessage = "This is a message from your server.";
                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.               
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Debug.LogError("Server sent his message - should be received by client");
                    stream.Close();
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }

    /// <summary>
    /// Connect to server and listen for the data incoming until it receive EOF
    /// </summary>
    /// <returns></returns>
    public IEnumerator ListenToData()
    {
              server = new TcpClient("localhost", networkManager.matchPort);
            Byte[] bytes = new Byte[1024];
            string serverMessage="";
        using (NetworkStream stream = server.GetStream())
        {
            while (!serverMessage.EndsWith("///"))
            {
            try
            {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while (stream.DataAvailable && (length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                    }
                
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
            yield return null;
        }
        }
        Debug.Log("Coroutine ended");

    }

}
