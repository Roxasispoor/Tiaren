using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch{
    public GameObject batchObject;
    public string material;
    public List<CombineInstance> combineInstances;
    public Batch(string material)
    {
        this.material = material;
        combineInstances = new List<CombineInstance>();
    }
}
