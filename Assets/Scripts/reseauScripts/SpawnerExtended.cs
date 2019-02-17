using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerExtended : SpawnerBehaviour {
    public void SetSpawnerIp(string address)
    {

        DefaultMachineIp = address;
    }
}
