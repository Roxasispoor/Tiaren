using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionToMasterFromInputField : ConnectionToMaster {
    // Use this for initialization
	public void SetMasterServerIp(string address)
    {

            ServerIp = address;
    }
    
}
