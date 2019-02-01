using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Transforms grid in jagged array for serialization
/// first step, saving number in prefab list 
/// second step, adding effects if needed
/// </summary>
public class JaggedGrid
{
    public int[] flagCoord;
    public int[] goalP1;
    public int[] goalP2;

    public int[] gridTable;
    
    public int[] spawnPlayerOne;
    public int[] spawnPlayerTwo;

    public int sizeX;
    public int sizeY;
    public int sizeZ;




    public JaggedGrid()
    {
    }

    public JaggedGrid(int sizeX, int sizeY, int sizeZ)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;
        gridTable = new int[sizeX * sizeY * sizeZ];
    }

    public virtual void ToJagged(Grid grid)
    {
        sizeX = grid.sizeX;
        sizeY = grid.sizeY;
        sizeZ = grid.sizeZ;

        gridTable = new int[grid.sizeX * grid.sizeY * grid.sizeZ];

        // y at last for potential futur need of compression
        for (int y = 0; y < grid.sizeY; y++)
        {
            for (int x = 0; x < grid.sizeX; x++)
            {

                for (int z = 0; z < grid.sizeZ; z++)
                {
                    if (grid.GridMatrix[x, y, z] == null)
                    {
                        this.gridTable[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = 0;

                    }
                    else
                    {
                        Placeable plac = grid.GridMatrix[x, y, z];
                        this.gridTable[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = plac.serializeNumber;

                    }

                }
            }
        }
    }

    public void SetCell(int value, int x, int y, int z)
    {
        this.gridTable[y * sizeZ * sizeX + z * sizeX + x] = value;
    }

    public void Save(string path = "Grid.json")
    {
        string text = JsonUtility.ToJson(this);

        File.WriteAllText(path, text);
    }

    public static JaggedGrid FillGridFromJSON(string path)
    {
        return JsonUtility.FromJson<JaggedGrid>(ReadString(path));
    }


    static string ReadString(string path = "Grid.json")
    {

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string toReturn = reader.ReadToEnd();
        reader.Close();
        return toReturn;
    }

    public List<Vector3Int> GetSpawnsP1()
    {
        List<Vector3Int> spawns = new List<Vector3Int>();
        for (int i = 0; i < spawnPlayerOne.Length; i += 3)
        {
            spawns.Add(new Vector3Int(spawnPlayerOne[i], spawnPlayerOne[i + 2], spawnPlayerOne[i + 1]));
        }
        return spawns;
    }

    public List<Vector3Int> GetSpawnsP2()
    {
        List<Vector3Int> spawns = new List<Vector3Int>();
        for (int i = 0; i < spawnPlayerTwo.Length; i += 3)
        {
            spawns.Add(new Vector3Int(spawnPlayerTwo[i], spawnPlayerTwo[i + 2], spawnPlayerTwo[i + 1]));
        }
        return spawns;
    }

    public List<Vector3Int> GetGoalsP1()
    {
        List<Vector3Int> goals = new List<Vector3Int>();
        for (int i = 0; i < goalP1.Length; i += 3)
        {
            goals.Add(new Vector3Int(goalP1[i], goalP1[i + 2], goalP1[i + 1]));
        }
        return goals;
    }

    public List<Vector3Int> GetGoalsP2()
    {
        List<Vector3Int> goals = new List<Vector3Int>();
        for (int i = 0; i < goalP2.Length; i += 3)
        {
            goals.Add(new Vector3Int(goalP2[i], goalP2[i + 2], goalP2[i + 1]));
        }
        return goals;
    }

    public Vector3Int GetFlagPos()
    {
        return new Vector3Int(flagCoord[0], flagCoord[2], flagCoord[1]);
    }
}
