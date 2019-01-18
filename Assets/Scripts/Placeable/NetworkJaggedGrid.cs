using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkJaggedGrid : JaggedGrid {

    public int[] netId;
    private List<Effect> attached;
    public string attachedEffectsString="";


    public override void ToJagged(Grid grid)
    {
        base.ToJagged(grid);
        netId = new int[grid.sizeX * grid.sizeY * grid.sizeZ];
        attached = new List<Effect>();

        // y at last for potential futur need of compression
        for (int y = 0; y < grid.sizeY; y++)
        {
            for (int x = 0; x < grid.sizeX; x++)
            {

                for (int z = 0; z < grid.sizeZ; z++)
                {
                    if (grid.GridMatrix[x, y, z] == null)
                    {
                        this.netId[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = 0;

                    }
                    else
                    {
                        this.netId[y * grid.sizeZ * grid.sizeX + z * grid.sizeX + x] = grid.GridMatrix[x, y, z].netId;
                        attached.AddRange(grid.GridMatrix[x, y, z].AttachedEffects);
                    }

                }
            }
        }
        attached.Add(new Damage(25));
        foreach(Effect effect in attached)
        {
            attachedEffectsString +=effect.Save() + "\n";
        }
    }

}
