using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTransparentCube : MonoBehaviour
{
    private static FactoryTransparentCube instance;

    private Queue<GameObject> freeTransparentCubes;
    private Transform parent;

    public static FactoryTransparentCube Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Factory");
                instance = go.AddComponent<FactoryTransparentCube>();
                instance.parent = instance.transform;
            }
            return instance;
        }
    }

    FactoryTransparentCube()
    {
        freeTransparentCubes = new Queue<GameObject>();
    }

    public GameObject getCube()
    {
        if (freeTransparentCubes.Count > 0)
        {
            GameObject cube =  freeTransparentCubes.Dequeue();
            cube.SetActive(true);
            return cube;
        } else
        {
            return Instantiate(GameManager.instance.prefabTransparentCube, parent);
        }
    }

    public void putBack(GameObject cube)
    {
        cube.SetActive(false);
        freeTransparentCubes.Enqueue(cube);
    }
}
