using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSelector : MonoBehaviour
{
    public LayerMask layerMask = 0;
    private int effectarea; //effectarea radius
    private int state;
    private SkillArea pattern;
    private List<Placeable> area;
    public Camera camera;

    public SkillArea Pattern
    {get {return pattern;} set {pattern = value;}}

    //public int State
    //{ get { return state; } set { state = value; } }

    public int EffectArea
    { get { return effectarea; } set { effectarea = value; } }

    public List<Placeable> Area
    { get { return area; } set { area = value; } }

    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
        layerMask = ~layerMask;
        state = 0;
        pattern = SkillArea.NONE;
        effectarea = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            state = (state == 3 ? 0 : state+1);
            //Debug.Log(state);
        }

        if (GameManager.instance.playingPlaceable != null || GameManager.instance.state == States.Spawn )
        {

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, layerMask))
            {
                if (hit.transform.GetComponent<Placeable>() != null)
                {
                    hit.transform.GetComponent<Placeable>().OnMouseOverWithLayer();

                    if (GameManager.instance.hovered != null)
                    {
                        if (area == null)
                            GameManager.instance.hovered.UnHighlight();
                        else
                        {
                            foreach (Placeable block in area)
                                block.UnHighlight();
                        }

                    }

                    GameManager.instance.hovered = hit.transform.GetComponent<Placeable>();

                    if (effectarea==0)
                    {
                        area = null;
   
                        if (GameManager.instance.hovered != null)
                        {
                            GameManager.instance.hovered.Highlight();
                        }
                    }
                    else
                    {
                        if (pattern == SkillArea.NONE)
                            area = Grid.instance.HighlightEffectArea(hit.transform.GetComponent<Placeable>(), effectarea);
                        else
                            area = Grid.instance.HighlightEffectArea(hit.transform.GetComponent<Placeable>(), effectarea, state, pattern);
                        foreach (Placeable block in area)
                            block.Highlight();
                    }
                }
            }
        }
    }
}
