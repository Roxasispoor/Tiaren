using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTotemEffect : CreateBlock
{
    public override void Use()
    {
        for (int i = 1; i < height + 1; i++)
        {
            //new CreateBlockRelativeEffect(Target, prefab, new Vector3Int(0, 1, 0), new Vector3Int(0, 1 + i, 0)).Use();
            if (!Skill.CheckConditionCreateOnPosition(Target.GetPosition() + face * i))
            {
                Debug.Log("CreateBlockEffect : Something on the way");
                break;
            }
            Debug.Log("CreatingblockEffect : Creating block");
            GameObject totem = Grid.instance.InstantiateCube(prefab, Target.GetPosition() + face * i);
            GameManager.instance.PlayingPlaceable.LinkedObjects.Add(totem.GetComponent<Totem>());
        }
    }

    public CreateTotemEffect(GameObject prefab, Vector3Int face, int height = 1) : base(prefab, face, height)
    {
    }
}
