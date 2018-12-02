﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapConverter : MonoBehaviour {

    List<CSVReader.BlockCSV> blocks;
    public static readonly int SPAWNPOINT = 99;
    private List<int> spawnPoints;


    public string inPath;
    public string outPath;

    public static Dictionary<string, int> strColorToSerialize = new Dictionary<string, int>
    {
        {"fbf236", SPAWNPOINT }, // Spawnpoint
        {"ffffff", 1 },
        {"000000", 0 },

    };

    public void ConvertGridFromText(string _inPath, string _outPath)
    {
        /*if (System.IO.File.Exists(_inPath))
        {
            Debug.Log("MapConverter: File " + _inPath + "does not exist");
            return;
        }*/

        Debug.Log("MapConverter: opening " + _inPath);

        blocks = CSVReader.ReadBlocks(_inPath);

        int minX = blocks[0].Position.x,
            minY = blocks[0].Position.y,
            minZ = blocks[0].Position.z,
            maxX = blocks[0].Position.x,
            maxY = blocks[0].Position.y,
            maxZ = blocks[0].Position.z;

        foreach (CSVReader.BlockCSV block in blocks)
        {
            Vector3Int position = block.Position;
            minX = Mathf.Min(minX, position.x);
            minY = Mathf.Min(minY, position.y);
            minZ = Mathf.Min(minZ, position.z);
            maxX = Mathf.Max(maxX, position.x);
            maxY = Mathf.Max(maxY, position.y);
            maxZ = Mathf.Max(maxZ, position.z);
        }

        int sizeX = maxX - minX + 1;
        int sizeY = maxY - minY + 1;
        int sizeZ = maxZ - minZ + 1;

        int shiftX = -1 * minX;
        int shiftY = -1 * minY;
        int shiftZ = -1 * minZ;

        int[] gridTable = new int[sizeX * sizeY * sizeZ]; // All elements are initialized to 0


        JaggedGrid jaggedGrid = new JaggedGrid(sizeX, sizeY, sizeZ);
        spawnPoints = new List<int>();

        foreach (CSVReader.BlockCSV block in blocks)
        {
            int x = block.Position.x + shiftX,
                y = block.Position.y + shiftY,
                z = block.Position.z + shiftZ;
            //gridTable[y * sizeZ * sizeX + z * sizeX + x] = strColorToSerialize[block.Type];
            if (strColorToSerialize[block.Type] == SPAWNPOINT)
            {
                //Debug.Log("SpawnPoints!!");
                spawnPoints.Add(x);
                spawnPoints.Add(z); // swapping z and y for Unity
                spawnPoints.Add(y); // swapping z and y for Unity
            } else
            {
                jaggedGrid.SetCell(strColorToSerialize[block.Type], x, y, z);
            }
        }
        
        jaggedGrid.SpawnPoints = spawnPoints.ToArray();
        jaggedGrid.Save(_outPath);
        Debug.Log("MapConverter: successfully converted to " + _outPath);
    }

    public void ConvertFromGUI()
    {
        if (inPath != "" && outPath != "")
        {
            ConvertGridFromText(inPath, outPath);
        }
    }
}
