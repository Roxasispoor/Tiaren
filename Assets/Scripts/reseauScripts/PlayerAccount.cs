using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;

public class PlayerAccount : MonoBehaviour {
    public AccountInfoPacket AccountInfoPacket;
    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
