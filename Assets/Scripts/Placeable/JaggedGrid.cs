using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


public class JaggedGrid:MonoBehaviour  {

   
    public String[] grille;

    public void ToJagged(Placeable[,,] grid)
    {
        grille= new String[grid.GetLength(0)* grid.GetLength(1)* grid.GetLength(2)];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z]==null)
                    {
                        this.grille[x * grid.GetLength(1) * grid.GetLength(2) + y * grid.GetLength(2) + z] = null;

                    }
                    else
                    {
                        this.grille[x * grid.GetLength(1) * grid.GetLength(2) + y * grid.GetLength(2) + z] = grid[x, y, z].GetType().ToString();

                    }
                   
                }
            }
        }
    }
    public void Save()
    {
        string blah = JsonUtility.ToJson(this);
      
        string path =  Application.dataPath + "Grid.json";
        Debug.Log("AssetPath:" + path);
        File.WriteAllText(path, blah);
    }
   
    public static JaggedGrid Load(string path)
    {
        var serializer = new XmlSerializer(typeof(JaggedGrid));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as JaggedGrid;
        }
    }
}
