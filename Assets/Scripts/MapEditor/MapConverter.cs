using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapConverter : MonoBehaviour {

    List<CSVReader.BlockCSV> blocks;
    public static readonly int SPAWNPOINTP1 = 99;
    public static readonly int SPAWNPOINTP2 = 98;
    public static readonly int FLAG = 97;
    public static readonly int GOALP1 = 96;
    public static readonly int GOALP2 = 95;
    private List<int> spawnPointsP1 = new List<int>();
    private List<int> spawnPointsP2 = new List<int>();
    private List<int> goalP1 = new List<int>();
    private List<int> goalP2 = new List<int>();
    private List<int> flag = new List<int>();


    public string inPath;
    public string outPath;

    public static Dictionary<string, int> strColorToSerialize = new Dictionary<string, int>
    {
        {"0000ff", SPAWNPOINTP1 }, // Spawnpoint
        {"ff0000", SPAWNPOINTP2 }, // Spawnpoint
        {"4444ff", GOALP1 }, // Goal player 1
        {"ff4444", GOALP2 }, // Goal player 2
        {"02cafa", 11 }, // Water
        {"838383", 10 }, // Stone
        {"f1d882", 9 }, // Sand
        {"0aa302", 8 }, // Grass
        {"926d02", 7 }, // Dirt
        {"bba34e", 6 }, // Bridge
        //{"fe00fe", 5 }, // Goal
        {"ffff00", FLAG }, // FlagParent
        {"ffffff", 1 }, // Basic cube (used for the 1st maps)
        {"000000", 0 }, // Empty

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

        foreach (CSVReader.BlockCSV block in blocks)
        {
            Debug.Log(string.Format("Bloc ({0}, {1}, {2}): {3} ", block.Position.x, block.Position.y, block.Position.z, block.Type));
            int x = block.Position.x + shiftX,
                y = block.Position.y + shiftY,
                z = block.Position.z + shiftZ;
            
            if (strColorToSerialize[block.Type] == SPAWNPOINTP1)
            {
                //Debug.Log("SpawnPoints!!");
                spawnPointsP1.Add(x);
                spawnPointsP1.Add(z); // swapping z and y for Unity
                spawnPointsP1.Add(y); // swapping z and y for Unity
            } else if (strColorToSerialize[block.Type] == SPAWNPOINTP2)
            {
                //Debug.Log("SpawnPoints!!");
                spawnPointsP2.Add(x);
                spawnPointsP2.Add(z); // swapping z and y for Unity
                spawnPointsP2.Add(y); // swapping z and y for Unity
            }
            else if (strColorToSerialize[block.Type] == GOALP1)
            {
                //Debug.Log("SpawnPoints!!");
                goalP1.Add(x);
                goalP1.Add(z); // swapping z and y for Unity
                goalP1.Add(y); // swapping z and y for Unity
            }
            else if (strColorToSerialize[block.Type] == GOALP2)
            {
                //Debug.Log("SpawnPoints!!");
                goalP2.Add(x);
                goalP2.Add(z); // swapping z and y for Unity
                goalP2.Add(y); // swapping z and y for Unity
            }else if (strColorToSerialize[block.Type] == FLAG)
            {
                //Debug.Log("SpawnPoints!!");
                flag.Add(x);
                flag.Add(z); // swapping z and y for Unity
                flag.Add(y); // swapping z and y for Unity
            }
            else {
                jaggedGrid.SetCell(strColorToSerialize[block.Type], x, y, z);
            }
        }
        
        jaggedGrid.spawnPlayerOne = spawnPointsP1.ToArray();
        jaggedGrid.spawnPlayerTwo = spawnPointsP2.ToArray();
        jaggedGrid.goalP1 = goalP1.ToArray();
        jaggedGrid.goalP2 = goalP2.ToArray();
        jaggedGrid.flagCoord = flag.ToArray();
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
