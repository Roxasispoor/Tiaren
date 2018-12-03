using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSelector : MonoBehaviour {
    public LayerMask layerMask=0;
    public Camera camera;
   
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,100000,layerMask ))
        {
            if(hit.transform.GetComponent<Placeable>()!=null)
            {
                hit.transform.GetComponent<Placeable>().OnMouseOverWithLayer();
            }
        }
          
    }
}
