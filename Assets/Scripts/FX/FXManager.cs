using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    private static FXManager instance;

    private Queue<GameObject> freeTransparentCubes;
    private Transform parent;

    private GameObject zipStart;
    private GameObject zipEnd;

    public static FXManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Factory");
                instance = go.AddComponent<FXManager>();
                instance.parent = instance.transform;
            }
            return instance;
        }
    }

    FXManager()
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

    public void ZiplinePreview(StandardCube target, LivingPlaceable launcher)
    {
        if (null == zipStart && null == zipEnd)
        {
            zipEnd = Instantiate(GameManager.instance.prefabTransparentZipline, target.GetPosition(), Quaternion.identity);
            zipStart = Instantiate(GameManager.instance.prefabTransparentZipline, launcher.GetPosition() + new Vector3Int(0, -1, 0), Quaternion.identity);
            zipStart.GetComponentInChildren<ZiplineFX>().ConnectZipline(zipEnd.GetComponentInChildren<ZiplineFX>());
        }
        else
        {
            zipStart.SetActive(true);
            zipEnd.SetActive(true);
            zipEnd.transform.position = target.GetPosition();
            zipStart.transform.position = launcher.GetPosition() + new Vector3Int(0, -1, 0);
        }
    }

    public void ZiplineUnpreview()
    {
        if (zipEnd != null && zipStart != null)
        {
            zipStart.SetActive(false);
            zipEnd.SetActive(false);
        }
    }
}
