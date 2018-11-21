using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class representing a cube in a path
/// </summary>

public class NodePath
{
    public int x, y, z;
    NodePath parent;

    private int distanceFromStart;

    public int DistanceFromStart
    {
        get
        {
            return distanceFromStart;
        }

        private set
        {
            distanceFromStart = value;
        }
    }

    public NodePath(int x, int y, int z, int distanceFromStart, NodePath parent)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.DistanceFromStart = distanceFromStart;
        this.parent = parent;
    }

    public static NodePath startPath(Vector3 pos)
    {
        return startPath((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public static NodePath startPath(int x, int y, int z)
    {
        return new NodePath(x, y - 1, z, 0, null);
    }

    public Vector3[] GetFullPath()
    {
        Vector3[] path = new Vector3[DistanceFromStart + 1];
        NodePath currentNode = this;
        for (int i = DistanceFromStart; i >= 0; i--)
        {
            path[i] = new Vector3(currentNode.x, currentNode.y, currentNode.z);
            currentNode = currentNode.parent;
        }
        return path;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() != typeof(NodePath))
        {
            return false;
        }
        NodePath other = (NodePath)obj;
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }
}