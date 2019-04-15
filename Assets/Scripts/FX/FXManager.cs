using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    /// <summary>
    /// Prefab used to create transparentCube.
    /// </summary>
    public GameObject prefabTransparentCube;
    /// <summary>
    /// Prefab used to preview the zipline
    /// </summary>
    public GameObject prefabTransparentZipline;
    /// <summary>
    /// Prefab used to show the grapple
    /// </summary>
    public GameObject prefabGrapple;
    /// <summary>
    /// Prefab used to preview the picots
    /// </summary>
    public GameObject prefabTransparentMakibishi;
    /// <summary>
    /// Material used for the previw of the creation of a cube.
    /// </summary>
    public Material materialPreviewCreate;
    /// <summary>
    /// Material used for the previw of the destruction of a cube.
    /// </summary>
    public Material materialPreviewDestroy;

    public static FXManager instance;

    private Queue<GameObject> freeTransparentCubes;

    private GameObject zipStart;
    private GameObject zipEnd;

    private GameObject makibishiPreview;

    private GameObject grapple;

    private void Awake()
    {
        // Singleton patern
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        freeTransparentCubes = new Queue<GameObject>();
    }

    public GameObject getCube()
    {
        if (freeTransparentCubes.Count > 0)
        {
            GameObject cube = freeTransparentCubes.Dequeue();
            cube.SetActive(true);
            return cube;
        }
        else
        {
            if(null != prefabTransparentCube)
                return Instantiate(prefabTransparentCube);
            else
            {
                Debug.LogError("the prefab is not there");
                return null;
            }
        }
    }

    public void putBack(GameObject cube)
    {
        cube.SetActive(false);
        freeTransparentCubes.Enqueue(cube);
    }

   
    /// <summary>
    /// Preview for the zipline
    /// </summary>
    /// <param name="target"></param>
    /// <param name="launcher"></param>
    public void ZiplinePreview(StandardCube target, LivingPlaceable launcher)
    {
        if (null == makibishiPreview)
        {
            zipEnd = Instantiate(prefabTransparentZipline, target.GetPosition(), Quaternion.identity);
            zipStart = Instantiate(prefabTransparentZipline, launcher.GetPosition() + new Vector3Int(0, -1, 0), Quaternion.identity);
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

    public void MakibishiPreview(StandardCube target)
    {
        if (null == makibishiPreview)
        {
            makibishiPreview = Instantiate(prefabTransparentMakibishi, target.GetPosition(), Quaternion.identity);
        }
        else
        {
            makibishiPreview.SetActive(true);
            makibishiPreview.transform.position = target.GetPosition();
        }
    }

    public void MakibishiUnpreview()
    {
        if (makibishiPreview != null)
        {
            makibishiPreview.SetActive(false);
        }
    }


    public void Grapplepreview(StandardCube target, Vector3Int offset)
    {
        Vector3 imagePosition = (Vector3)offset * 0.5f;
        
        if(null == grapple)
        {
            grapple = Instantiate(prefabGrapple, target.GetPosition() + imagePosition, Quaternion.identity);
            grapple.GetComponentInChildren<MeshRenderer>().material = materialPreviewCreate;
            grapple.GetComponent<GrappleFX>().ConnectRope(materialPreviewCreate);
        }
        else
        {
            grapple.SetActive(true);
            grapple.transform.position = target.GetPosition() + imagePosition;
        }
    }

    public void GrappleUnpreview()
    {
        if (null != grapple)
            grapple.SetActive(false);
    }
}
