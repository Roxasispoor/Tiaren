using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{

    public static string[] ReadLines(string path)
    {
        //Debug.Log(path);
        string fileData = System.IO.File.ReadAllText(path);
        string[] lines = fileData.Split('\n');
        return lines;
    }

    public static List<Vector3> ReadVectors(string path, char separator = ' ')
    {
        List<Vector3> vectors = new List<Vector3>();
        foreach (string line in ReadLines(path))
        {
            if (line == "")
                continue;
            //Debug.Log("line: "+  line);
            string[] splitted = line.Split(separator);
            if (splitted[0] == "" || splitted[0] == "#")
            {
                continue;
            }
            Vector3 newVector = new Vector3(int.Parse(splitted[0]), int.Parse(splitted[2]), int.Parse(splitted[1]));
            vectors.Add(newVector);
            //Debug.Log(newVector);
        }
        return vectors;
    }

    public class BlockCSV
    {
        Vector3Int position;
        string type;

        public BlockCSV(Vector3Int _position, string _type)
            {
                position = _position;
                type = _type;
            }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public Vector3Int Position
        {
            get
            {
                return position;
            }
        }
    }

    public static List<BlockCSV> ReadBlocks(string path, char separator = ' ')
    {
        List<BlockCSV> blocks = new List<BlockCSV>();
        foreach (string line in ReadLines(path))
        {
            if (line == "")
                continue;
            //Debug.Log("line: " + line);
            string[] splitted = line.Split(separator);
            if (splitted[0] == "" || splitted[0] == "#")
            {
                continue;
            }
            Vector3Int newVector = new Vector3Int(int.Parse(splitted[0]), int.Parse(splitted[2]), int.Parse(splitted[1]));
            BlockCSV newBlock = new BlockCSV(newVector, splitted[3].Remove(6));
            blocks.Add(newBlock);
            //Debug.Log(newVector);
        }
        return blocks;
    }
}