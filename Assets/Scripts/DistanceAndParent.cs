﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Que représente cette classe ?
/// </summary>
public class DistanceAndParent{

    private int distance;
    private int x;
    private int y;
    private int z;


    /// <summary>
    /// Le parent est lui meme un objet de ce type ? Oui.
    /// </summary>
    private DistanceAndParent parent;
    
    public DistanceAndParent(int x, int y, int z)
    {
        this.distance = -1;
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public DistanceAndParent(int distance, DistanceAndParent parent)
    {
        this.distance = distance;
        this.parent = parent;

    }
    public int GetDistance()
    {
        return distance;
    }
    public DistanceAndParent GetParent()
    {
        return parent;
    }
    public void SetDistance(int distance)
    {
        this.distance = distance;
    }
    public void SetParent(DistanceAndParent parent)
    {
        this.parent = parent;
    }
}
