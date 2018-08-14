using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Classe qui transforme la grille en jagged array a des fins de serialization. 
/// La serialization se fait en deux temps: On sauvegarde le numero dans la liste préfab correspondant
/// On ajoute des effets en plus si nécessaires.
/// </summary>
public class JaggedGrid
{


    public int[] grille;

    public void ToJagged(Grille grid)
    {
        grille = new int[grid.sizeX * grid.sizeY * grid.sizeZ];

        //On met y en dernier pour des besoins futurs potentiels de compression
        for (int y = 0; y < grid.sizeY; y++)
        {
            for (int x = 0; x < grid.sizeX; x++)
            {

                for (int z = 0; z < grid.sizeZ; z++)
                {
                    if (grid.Grid[x, y, z] == null)
                    {
                        this.grille[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = 0;

                    }
                    else
                    {
                        this.grille[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = grid.Grid[x, y, z].serializeNumber;

                    }

                }
            }
        }
    }
    public void Save()
    {
        string blah = JsonUtility.ToJson(this);

        string path = "Grid.json";
        Debug.Log("AssetPath:" + path);
        File.WriteAllText(path, blah);
    }

    public static JaggedGrid FillGridFromJSON()
    {
        return JsonUtility.FromJson<JaggedGrid>(ReadString());
    }


    static string ReadString()
    {
        string path = "Grid.json";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string toreturn = reader.ReadToEnd();
        reader.Close();
        return toreturn;
    }

}
