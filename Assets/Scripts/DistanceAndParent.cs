using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe utile pour le pathfinding et la remontée de chemin
/// </summary>
public class DistanceAndParent
{

    private int distance;
    private Vector3Int pos;

    private DistanceAndParent parent;
    private Color color;

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
        }
    }

    public Vector3Int Pos
    {
        get
        {
            return pos;
        }

        set
        {
            pos = value;
        }
    }

    public DistanceAndParent(int x, int y, int z)
    {
        this.distance = -1;
        this.Pos = new Vector3Int(x, y, z);
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
