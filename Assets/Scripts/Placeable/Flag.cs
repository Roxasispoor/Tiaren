using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : ObjectOnBloc {

    protected override void Awake()
    {
        base.Awake();
        isPickable=true;
        isPicked=false;
        dropOnDeathPicker=true;
        dropOnDeathBlocUnder=true;
    }
    public override void SomethingPutAbove()
    {
        Debug.Log("raise!");
         Vector3Int currentPos = GetPosition();
        while (Grid.instance.GridMatrix[currentPos.x, currentPos.y, currentPos.z] != null && currentPos.y < Grid.instance.sizeY)
        {
            transform.SetParent(Grid.instance.GridMatrix[currentPos.x, currentPos.y, currentPos.z].transform.Find("Inventory"));
            transform.localPosition = new Vector3();
            currentPos = new Vector3Int(currentPos.x, currentPos.y + 1, currentPos.z);
        }

       
    }
}
