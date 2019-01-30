using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineFX : MonoBehaviour
{

    private ZiplineFX startZip;
    private ZiplineFX endZip;
    private LineRenderer rope;
    [SerializeField]
    private Transform topPole;
    [SerializeField]
    private Material ropeMaterial;

    public Transform TopPole
    {
        get
        {
            return topPole;
        }
    }

    public ZiplineFX StartZipline
    {
        set
        {
            startZip = value;
        }
    }

    public ZiplineFX endZipline
    {
        set
        {
            endZip = value;
        }
    }

    public LineRenderer Rope
    {
        set
        {
            rope = value;
        }
    }

    /// <summary>
    /// Call it from the start of the zipline to create the rope
    /// </summary>
    /// <param name="endZipline"></param>
    public void ConnectZipline(ZiplineFX endZipline)
    { //new Color (173,132,69)
        startZip = this;
        endZipline.StartZipline = this;
        endZipline.endZip = endZipline;
        endZip = endZipline;
        rope = gameObject.AddComponent<LineRenderer>();
        endZip.rope = rope;
        rope.material = ropeMaterial;
        rope.widthMultiplier = 0.02f;
        rope.positionCount = 2;
        Vector3[] topPositions = new Vector3[] { startZip.TopPole.position, endZip.TopPole.position };
        rope.SetPositions(topPositions);
    }


    // TODO : Create a function andcall it only when needed
    public void Update()
    {
        // To call only once
        if (this == startZip)
        {

            Vector3[] topPositions = new Vector3[] { startZip.TopPole.position, endZip.TopPole.position };
            rope.SetPositions(topPositions);
        }
    }
}


