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
        if(GameManager.instance.playingPlaceable!=null || GameManager.instance.state == States.Spawn)
        { 
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,100000,layerMask ))
        {
            if(hit.transform.GetComponent<Placeable>()!=null)
            {
                hit.transform.GetComponent<Placeable>().OnMouseOverWithLayer();
               
                if(GameManager.instance.hovered !=null )
                { 
                     GameManager.instance.hovered.UnHighlight();

                }
                GameManager.instance.hovered = hit.transform.GetComponent<Placeable>();
                if (GameManager.instance.hovered != null)
                {
                    GameManager.instance.hovered.Highlight();
                }

            }
        }
        }
    }
}
